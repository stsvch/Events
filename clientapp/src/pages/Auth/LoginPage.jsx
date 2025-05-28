import React, { useState } from 'react';
import { TextField, Button, Card, Typography, Box } from '@mui/material';
import { useAuth } from '../../hooks/useAuth';
import { useNavigate, useLocation, Link } from 'react-router-dom';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || '/';

  const [form, setForm] = useState({ username: '', password: '' });
  const [error, setError] = useState('');

  const handleChange = e => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async e => {
    e.preventDefault();
    try {
      await login(form);
      // Navigate to the original destination or home, replacing history
      navigate(from, { replace: true });
    } catch (err) {
      setError(err.message || 'Login failed');
    }
  };

  return (
    <Box display="flex" justifyContent="center" mt={8}>
      <Card sx={{ p: 4, maxWidth: 400, width: '100%' }}>
        <Typography variant="h5" mb={2}>Login</Typography>
        {error && (
          <Typography color="error" mb={2}>{error}</Typography>
        )}
        <Box component="form" onSubmit={handleSubmit} display="grid" gap={2}>
          <TextField
            label="Username"
            name="username"
            value={form.username}
            onChange={handleChange}
            fullWidth
            required
          />
          <TextField
            label="Password"
            type="password"
            name="password"
            value={form.password}
            onChange={handleChange}
            fullWidth
            required
          />
          <Button type="submit" variant="contained" color="primary" fullWidth>
            Login
          </Button>
        </Box>
        <Typography variant="body2" mt={2} align="center">
          Don't have an account? <Link to="/register">Register</Link>
        </Typography>
      </Card>
    </Box>
  );
}