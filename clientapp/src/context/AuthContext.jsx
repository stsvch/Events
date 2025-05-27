// src/context/AuthContext.jsx
import React, { createContext, useState, useEffect, useCallback } from 'react';
import { register as apiRegister, login as apiLogin, refreshToken as apiRefresh, logout as apiLogout } from '../api/auth';
import axios from '../api/axiosInstance';
import { jwtDecode } from 'jwt-decode';

export const AuthContext = createContext({
  user: null,
  accessToken: null,
  register: async () => {},
  login: async () => {},
  logout: () => {},
});

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [accessToken, setAccessToken] = useState(null);
  const [refresh, setRefresh] = useState(null);

  // 1) Сначала объявляем logout, потому что он используется в handleRefresh и эффектах
  const logout = useCallback(async () => {
    try {
      if (refresh) {
        await apiLogout(refresh);
      }
    } catch {
      // ignore
    } finally {
      setUser(null);
      setAccessToken(null);
      setRefresh(null);
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      window.location.href = '/login';
    }
  }, [refresh]);

  // 2) Затем объявляем handleRefresh
  const handleRefresh = useCallback(async refreshToken => {
    try {
      const { data } = await apiRefresh(refreshToken);
      setAccessToken(data.accessToken);
      setRefresh(data.refreshToken);
      localStorage.setItem('accessToken', data.accessToken);
      localStorage.setItem('refreshToken', data.refreshToken);
    } catch {
      logout();
    }
  }, [logout]);

  // Утилита для извлечения массива ролей из payload
  const extractRoles = payload => {
    const raw =
      payload.role ??
      payload.roles ??
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ??
      [];
    return Array.isArray(raw) ? raw : [raw];
  };

  // 3) Интерцептор на 401 — автоматический рефреш
  useEffect(() => {
    const interceptor = axios.interceptors.response.use(
      res => res,
      async err => {
        if (err.response?.status === 401 && refresh) {
          try {
            const { data } = await apiRefresh(refresh);
            setAccessToken(data.accessToken);
            setRefresh(data.refreshToken);
            localStorage.setItem('accessToken', data.accessToken);
            localStorage.setItem('refreshToken', data.refreshToken);

            err.config.headers.Authorization = `Bearer ${data.accessToken}`;
            return axios(err.config);
          } catch {
            logout();
          }
        }
        return Promise.reject(err);
      }
    );
    return () => axios.interceptors.response.eject(interceptor);
  }, [refresh, logout]);

  // 4) При монтировании подхватим токены и при необходимости обновим
  useEffect(() => {
    const savedToken = localStorage.getItem('accessToken');
    const savedRefresh = localStorage.getItem('refreshToken');
    if (savedToken && savedRefresh) {
      setAccessToken(savedToken);
      setRefresh(savedRefresh);
      handleRefresh(savedRefresh);
    }
  }, [handleRefresh]);

  // 5) При инициализации подтягиваем user из токена (если есть)
  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      const payload = jwtDecode(token);
      setUser({
        id: payload.sub,
        username: payload.unique_name,
        roles: extractRoles(payload),
      });
    }
  }, []);

  // 6) Регистрация и логин
  const handleRegister = async regData => {
    const { data } = await apiRegister(regData);
    if (!data.succeeded) {
      throw new Error(data.errors.join(', '));
    }

    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    setAccessToken(data.accessToken);
    setRefresh(data.refreshToken);

    const payload = jwtDecode(data.accessToken);
    setUser({
      id: payload.sub,
      username: payload.unique_name,
      roles: extractRoles(payload),
    });
  };

  const handleLogin = async ({ username, password }) => {
    const { data } = await apiLogin({ username, password });
    if (!data.succeeded) {
      throw new Error(data.errors.join(', '));
    }

    localStorage.setItem('accessToken', data.accessToken);
    localStorage.setItem('refreshToken', data.refreshToken);
    setAccessToken(data.accessToken);
    setRefresh(data.refreshToken);

    const payload = jwtDecode(data.accessToken);
    setUser({
      id: payload.sub,
      username: payload.unique_name,
      roles: extractRoles(payload),
    });
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        accessToken,
        register: handleRegister,
        login: handleLogin,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};
