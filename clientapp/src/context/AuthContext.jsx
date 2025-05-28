import React, { createContext, useState, useEffect, useCallback, useMemo, useContext } from 'react';
import {
  register as apiRegister,
  login as apiLogin,
  refreshToken as apiRefresh,
  logout as apiLogout,
} from '../api/auth';
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
  const [refreshToken, setRefreshToken] = useState(null);

  useEffect(() => {
    const storedAT = localStorage.getItem('accessToken');
    const storedRT = localStorage.getItem('refreshToken');
    if (storedAT && storedRT) {
      setAccessToken(storedAT);
      setRefreshToken(storedRT);
      axios.defaults.headers.common['Authorization'] = `Bearer ${storedAT}`;

      let decoded = {};
      try {
        decoded = jwtDecode(storedAT);
      } catch (e) {
        console.error('Failed to decode token:', e);
      }
      const rawRoles =
        decoded.role ||
        decoded.roles ||
        decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
        [];
      const roles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
      setUser({
        id: decoded.sub,
        name: decoded.unique_name,
        roles,
      });
    }
  }, []);

  const handleRegister = useCallback(async (data) => {
    const res = await apiRegister(data);
    return res.data;
  }, []);

  const handleLogin = useCallback(async ({ username, password }) => {
    const { data } = await apiLogin({ username, password });
    const { accessToken: at, refreshToken: rt } = data;

    setAccessToken(at);
    setRefreshToken(rt);
    axios.defaults.headers.common['Authorization'] = `Bearer ${at}`;
    localStorage.setItem('accessToken', at);
    localStorage.setItem('refreshToken', rt);

    let decoded = {};
    try {
      decoded = jwtDecode(at);
    } catch (e) {
      console.error('Failed to decode token:', e);
    }
    const rawRoles =
      decoded.role ||
      decoded.roles ||
      decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
      [];
    const roles = Array.isArray(rawRoles) ? rawRoles : [rawRoles];
    setUser({
      id: decoded.sub,
      name: decoded.unique_name,
      roles,
    });

    return data;
  }, []);

  const handleLogout = useCallback(async () => {
    try {
      if (refreshToken) await apiLogout(refreshToken);
    } catch {}
    setUser(null);
    setAccessToken(null);
    setRefreshToken(null);
    delete axios.defaults.headers.common['Authorization'];
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
  }, [refreshToken]);

  useEffect(() => {
    const interceptor = axios.interceptors.response.use(
      (response) => response,
      async (error) => {
        const original = error.config;
        if (
          error.response?.status === 401 &&
          refreshToken &&
          !original._retry &&
          !original.url.includes('/auth/refresh')
        ) {
          original._retry = true;
          try {
            const { data } = await apiRefresh(refreshToken);
            const { accessToken: newAT, refreshToken: newRT } = data;
            // update tokens
            setAccessToken(newAT);
            setRefreshToken(newRT);
            localStorage.setItem('accessToken', newAT);
            localStorage.setItem('refreshToken', newRT);
            axios.defaults.headers.common['Authorization'] = `Bearer ${newAT}`;
            original.headers['Authorization'] = `Bearer ${newAT}`;
            return axios(original);
          } catch {
            handleLogout();
            return Promise.reject(error);
          }
        }
        return Promise.reject(error);
      }
    );
    return () => {
      axios.interceptors.response.eject(interceptor);
    };
  }, [refreshToken, handleLogout]);

  // Memoize context value
  const authContextValue = useMemo(
    () => ({
      user,
      accessToken,
      register: handleRegister,
      login: handleLogin,
      logout: handleLogout,
    }),
    [user, accessToken, handleRegister, handleLogin, handleLogout]
  );

  return (
    <AuthContext.Provider value={authContextValue}>
      {children}
    </AuthContext.Provider>
  );
};

export function useAuth() {
  return useContext(AuthContext);
}
