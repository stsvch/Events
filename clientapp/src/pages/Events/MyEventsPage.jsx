// src/pages/Events/MyEventsPage.jsx
import React from 'react';
import {
  Box,
  Typography,
  Grid,
  CircularProgress,
  Alert,
  Button,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { useQuery, useQueries } from '@tanstack/react-query';

import { getMyEvents } from '../../api/events';
import { getFirstEventImage } from '../../api/images';
import EventCard from '../../components/EventCard';

export default function MyEventsPage() {
  const {
    data: events = [],
    isLoading: loadingEvents,
    isError: eventsError,
    refetch: refetchEvents,
  } = useQuery({
    queryKey: ['myEvents'],
    queryFn: () => getMyEvents().then(res => res.data),
  });

  const imageQueries = useQueries({
    queries: events.map(evt => ({
      queryKey: ['firstImage', evt.id],
      queryFn: () => getFirstEventImage(evt.id),
      enabled: events.length > 0,
      placeholderData: '/images/placeholder.png',
      onError: err => {
        if (err.response?.status !== 404) console.error(err);
      },
    })),
  });

  if (loadingEvents) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }

  if (eventsError) {
    return (
      <Box mt={4}>
        <Alert
          severity="error"
          action={
            <Button onClick={refetchEvents}>
              Retry
            </Button>
          }
        >
          Failed to load your events.
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
        {events.map((evt, idx) => {
          const imgQ = imageQueries[idx];
          const loadingImg = imgQ.isLoading;
          const errorImg = imgQ.isError;
          const imageUrl = imgQ.data || '/images/placeholder.png';

          return (
            <Grid item xs={12} sm={6} md={4} key={evt.id}>
              {loadingImg ? (
                <Box display="flex" justifyContent="center" p={4}>
                  <CircularProgress />
                </Box>
              ) : errorImg ? (
                <Alert severity="error">Error loading image</Alert>
              ) : (
                <EventCard evt={evt} imageUrl={imageUrl} />
              )}
            </Grid>
          );
        })}
      </Grid>
    </Box>
  );
}
