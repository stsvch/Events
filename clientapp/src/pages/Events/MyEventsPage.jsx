// src/pages/Events/MyEventsPage.jsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardActionArea,
  CardContent,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { getParticipants, unregisterParticipant } from '../../api/participants';
import { getEventById } from '../../api/events';

export default function MyEventsPage() {
  const [events, setEvents] = useState([]); // [{ registration, event }, …]
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  const fetchMyEvents = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      // 1) get registrations
      const { data: regs } = await getParticipants();
      // 2) fetch each event’s details
      const details = await Promise.all(
        regs.map(async reg => {
          const { data: evt } = await getEventById(reg.eventId);
          return { registration: reg, event: evt };
        })
      );
      setEvents(details);
    } catch (err) {
      setError(err.message || 'Failed to load your events');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    fetchMyEvents();
  }, [fetchMyEvents]);

  const handleUnregister = async (registrationId, eventId) => {
    if (!window.confirm('Are you sure you want to unregister?')) return;
    try {
      await unregisterParticipant(registrationId, eventId);
      await fetchMyEvents();
    } catch (err) {
      alert(err.message || 'Failed to unregister');
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box mt={4}>
        <Alert
          severity="error"
          action={
            <Button color="inherit" size="small" onClick={fetchMyEvents}>
              Retry
            </Button>
          }
        >
          {error}
        </Alert>
      </Box>
    );
  }

  if (events.length === 0) {
    return (
      <Box textAlign="center" mt={8}>
        <Typography>You haven’t registered for any events yet.</Typography>
        <Button
          component={Link}
          to="/events"
          variant="contained"
          sx={{ mt: 2 }}
        >
          Browse all events
        </Button>
      </Box>
    );
  }

  return (
    <Box p={4}>
      <Typography variant="h4" gutterBottom>
        My Events
      </Typography>

      <Grid container spacing={2}>
        {events.map(({ registration, event }) => (
          <Grid item xs={12} sm={6} md={4} key={registration.id}>
            <Card>
              <CardActionArea component={Link} to={`/events/${event.id}`}>
                <CardContent>
                  <Typography variant="h6">{event.title}</Typography>
                  <Typography variant="body2" color="textSecondary">
                    {new Date(event.date).toLocaleString()} — {event.venue}
                  </Typography>
                  <Typography
                    variant="body2"
                    color="textSecondary"
                    sx={{ mt: 1 }}
                  >
                    Registered on:{' '}
                    {new Date(
                      registration.dateOfRegistration
                    ).toLocaleDateString()}
                  </Typography>
                </CardContent>
              </CardActionArea>
              <Box display="flex" justifyContent="flex-end" p={2}>
                <Button
                  variant="contained"
                  color="warning"
                  onClick={() =>
                    handleUnregister(registration.id, event.id)
                  }
                >
                  Unregister
                </Button>
              </Box>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}
