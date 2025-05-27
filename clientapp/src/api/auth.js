import api from './axiosInstance';

export function register(data) {
  return api.post('/auth/register', {
    username: data.username,
    email: data.email,
    password: data.password,
    firstName: data.firstName,
    lastName: data.lastName,
    dateOfBirth: data.dateOfBirth,
  });
}

export function login({ username, password }) {
  return api.post('/auth/login', { username, password });
}

export function refreshToken(refreshToken) {
  return api.post('/auth/refresh', { refreshToken });
}

export function logout(refreshToken) {
  return api.post('/auth/logout', { refreshToken });
}