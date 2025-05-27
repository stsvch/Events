// src/utils/PrivateRoute.jsx
import React from 'react';
import { Route, Redirect } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

// roles: массив строк, напр. ['Admin', 'RegisteredUser']
function PrivateRoute({ component: Component, roles = [], ...rest }) {
  const { user, accessToken } = useAuth();

  return (
    <Route
      {...rest}
      render={props => {
        if (!accessToken) {
          return <Redirect to={{ pathname: '/login', state: { from: props.location } }} />;
        }
        if (
          roles.length > 0 &&
          (!user?.roles || !roles.some(r => user.roles.includes(r)))
        ) {
          return <Redirect to="/unauthorized" />;
        }
        return <Component {...props} />;
      }}
    />
  );
}

export default PrivateRoute;
