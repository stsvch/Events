// src/pages/Events/RegisterParticipantPage.jsx
import React from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  TextField,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import { useAuth } from '../../context/AuthContext';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getCurrentParticipant,
  registerParticipant,
} from '../../api/participants';

export default function RegisterParticipantPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const eventId = searchParams.get('eventId');
  const { user } = useAuth();
  const queryClient = useQueryClient();

  const {
    data: participant,
    isLoading,
    error,
  } = useQuery({
    queryKey: ['currentParticipant'],
    queryFn: () => getCurrentParticipant().then(res => res.data),
    enabled: !!user && !!eventId,
  });

  const mutation = useMutation({
    mutationFn: () => {
      if (!eventId) {
        return Promise.reject(new Error('Missing eventId'));
      }
      return registerParticipant({ eventId });
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['isRegistered', eventId] });
      queryClient.invalidateQueries({ queryKey: ['participants', eventId] });
      queryClient.invalidateQueries({ queryKey: ['event', eventId] });
      navigate(`/events/${eventId}`);
    },
  });

  if (!eventId) {
    return <Alert severity="error">Event ID is missing in URL</Alert>;
  }

  if (isLoading) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Alert severity="error">
          {error.message ?? 'An unexpected error occurred.'}
        </Alert>
      </Container>
    );
  }

  return (
    <Container maxWidth="sm" sx={{ mt: 4 }}>
      <Typography variant="h5" gutterBottom>
        Confirm Your Registration
      </Typography>

      <Box component="form" noValidate>
        <TextField
          label="Full Name"
          value={participant.fullName}
          fullWidth
          margin="normal"
          InputProps={{ readOnly: true }}
        />
        <TextField
          label="Email"
          value={participant.email}
          fullWidth
          margin="normal"
          InputProps={{ readOnly: true }}
        />
        <TextField
          label="Date of Birth"
          value={new Date(participant.dateOfBirth).toLocaleDateString()}
          fullWidth
          margin="normal"
          InputProps={{ readOnly: true }}
        />

        <Box display="flex" justifyContent="space-between" mt={3}>
          <Button
            variant="contained"
            color="primary"
            onClick={() => mutation.mutate()}
            disabled={mutation.isLoading}
          >
            {mutation.isLoading ? (
              <CircularProgress size={24} />
            ) : (
              'Confirm'
            )}
          </Button>
          <Button variant="outlined" onClick={() => navigate(-1)}>
            Cancel
          </Button>
        </Box>
      </Box>
    </Container>
  );
}
