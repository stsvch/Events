// src/components/Layout.jsx
import React, { useState } from 'react';
import { AppBar, Toolbar, IconButton, Typography, Button, Drawer, List, ListItem, ListItemText, Box } from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import { Link, useHistory } from 'react-router-dom';
import { useAuth } from '../hooks/useAuth';

export default function Layout({ children }) {
  const [drawerOpen, setDrawerOpen] = useState(false);
  const { user, logout } = useAuth();
  const history = useHistory();

  const handleLogout = () => {
    logout();           // вызывает контекстный logout
    // history.push('/login'); — уже есть редирект внутри logout
  };

  const navItems = [
    { text: 'Events',    path: '/events' },
    { text: 'My Events', path: '/my-events', role: 'RegisteredUser' },
    { text: 'Admin',     path: '/admin/events', role: 'Admin' },
  ];

  return (
    <>
      <AppBar position="static">
        <Toolbar>
          <IconButton edge="start" color="inherit" onClick={() => setDrawerOpen(o => !o)} sx={{ mr: 2 }}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>EventApp</Typography>

          {user
            ? <Button color="inherit" onClick={handleLogout}>Logout</Button>
            : <>
                <Button color="inherit" component={Link} to="/login">Login</Button>
                <Button color="inherit" component={Link} to="/register">Register</Button>
              </>
          }
        </Toolbar>
      </AppBar>

      <Drawer open={drawerOpen} onClose={() => setDrawerOpen(false)}>
        <Box sx={{ width: 240 }} role="presentation" onClick={() => setDrawerOpen(false)}>
          <List>
            {navItems.map(item =>
              (!item.role || user?.roles.includes(item.role)) && (
                <ListItem button key={item.text} component={Link} to={item.path}>
                  <ListItemText primary={item.text} />
                </ListItem>
              )
            )}
          </List>
        </Box>
      </Drawer>

      <Box component="main" sx={{ mt: 2, p: 3 }}>
        {children}
      </Box>
    </>
  );
}
