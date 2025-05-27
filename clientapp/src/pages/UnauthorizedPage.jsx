// src/pages/Misc/UnauthorizedPage.jsx
import React from 'react';
import { Box, Typography, Button } from '@mui/material';
import { Link } from 'react-router-dom';

export default function UnauthorizedPage() {
  return (
    <Box textAlign="center" mt={8}>
      <Typography variant="h4" gutterBottom>
        403 — Unauthorized
      </Typography>
      <Typography variant="body1" gutterBottom>
        You do not have permission to view this page.
      </Typography>
      <Button component={Link} to="/" variant="contained">
        Go to Home
      </Button>
    </Box>
  );
}
