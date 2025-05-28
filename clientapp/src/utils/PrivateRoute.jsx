// src/utils/PrivateRoute.jsx
import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function PrivateRoute({ roles = [] }) {
  const { user, accessToken } = useAuth();
  const location = useLocation();

console.log('PrivateRoute: user.roles =', user?.roles);

  if (!accessToken) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (
    roles.length > 0 &&
    (!user?.roles || !roles.some(role => user.roles.includes(role)))
  ) {
    return <Navigate to="/unauthorized" replace />;
  }

  return <Outlet />;
}
