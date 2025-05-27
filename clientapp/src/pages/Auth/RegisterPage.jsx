import React, { useState } from 'react';
import { TextField, Button, Card, Typography, Box } from '@mui/material';
import { useAuth } from '../../hooks/useAuth';
import { useHistory, Link } from 'react-router-dom';

export default function RegisterPage() {
  const { register } = useAuth();
  const history = useHistory();
  const [form, setForm] = useState({
    username: '',
    firstName: '',
    lastName: '',
    email: '',
    dateOfBirth: '',
    password: '',
    confirmPassword: ''
  });
  const [error, setError] = useState('');

  const handleChange = e => setForm(f => ({ ...f, [e.target.name]: e.target.value }));

  const handleSubmit = async e => {
    e.preventDefault();
    if (form.password !== form.confirmPassword) {
      setError('Passwords do not match');
      return;
    }
    try {
      await register({
        username: form.username,
        email: form.email,
        password: form.password,
        firstName: form.firstName,
        lastName: form.lastName,
        dateOfBirth: form.dateOfBirth,
      });
      history.push('/events');
    } catch (err) {
      setError(err.message || 'Registration failed');
    }
  };

  return (
    <Box display="flex" justifyContent="center" mt={8}>
      <Card sx={{ p: 4, maxWidth: 500, width: '100%' }}>
        <Typography variant="h5" mb={2}>Register</Typography>
        {error && (
          <Typography color="error" mb={2}>{error}</Typography>
        )}
        <Box component="form" onSubmit={handleSubmit} display="grid" gap={2}>
          <TextField label="Username" name="username" value={form.username} onChange={handleChange} fullWidth required />
          <TextField label="First Name" name="firstName" value={form.firstName} onChange={handleChange} fullWidth required />
          <TextField label="Last Name" name="lastName" value={form.lastName} onChange={handleChange} fullWidth required />
          <TextField label="Email" name="email" type="email" value={form.email} onChange={handleChange} fullWidth required />
          <TextField
            label="Date of Birth"
            name="dateOfBirth"
            type="date"
            value={form.dateOfBirth}
            onChange={handleChange}
            InputLabelProps={{ shrink: true }}
            fullWidth
            required
          />
          <TextField label="Password" name="password" type="password" value={form.password} onChange={handleChange} fullWidth required />
          <TextField label="Confirm Password" name="confirmPassword" type="password" value={form.confirmPassword} onChange={handleChange} fullWidth required />
          <Button type="submit" variant="contained" color="primary" fullWidth>Register</Button>
        </Box>
        <Typography variant="body2" mt={2} align="center">
          Already have an account? <Link to="/login">Login</Link>
        </Typography>
      </Card>
    </Box>
  );
}