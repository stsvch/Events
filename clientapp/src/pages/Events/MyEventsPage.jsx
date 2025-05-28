// src/pages/Events/MyEventsPage.jsx
import React from 'react';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardActionArea,
  CardContent,
  CardMedia,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { useQuery, useQueries, useMutation, useQueryClient } from '@tanstack/react-query';

import { getParticipants, unregisterParticipant } from '../../api/participants';
import { getEventById } from '../../api/events';
import { getFirstEventImage } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';

export default function MyEventsPage() {
  const queryClient = useQueryClient();

  // 1) Загрузка ваших регистраций
  const { data: regs = [], isLoading: loadingRegs, isError: regsError } = useQuery(
    ['myRegistrations'],
    () => getParticipants().then(res => res.data)
  );

  // 2) Параллельная загрузка деталей каждого события
  const eventQueries = useQueries({
    queries: regs.map(reg => ({
      queryKey: ['event', reg.eventId],
      queryFn: () => getEventById(reg.eventId).then(res => res.data),
      enabled: !!regs.length,
      staleTime: 5 * 60_000,
    }))
  });

  // 3) Параллельная загрузка первого фото каждого события
  const imageQueries = useQueries({
    queries: regs.map(reg => ({
      queryKey: ['event', reg.eventId, 'firstImage'],
      queryFn: () => getFirstEventImage(reg.eventId),
      enabled: !eventQueries.find(q => q.isLoading),
      placeholderData: PLACEHOLDER_IMAGE,
      onError: err => {
        if (err.response?.status !== 404) console.error(err);
      }
    }))
  });

  // 4) Мутация отмены регистрации
  const unregisterMutation = useMutation(
    ({ registrationId, eventId }) => unregisterParticipant(registrationId, eventId),
    {
      onSuccess: () => {
        // после удачного снятия — переспросим и регистрации, и связанные события
        queryClient.invalidateQueries(['myRegistrations']);
        regs.forEach(reg => {
          queryClient.invalidateQueries(['event', reg.eventId]);
          queryClient.invalidateQueries(['event', reg.eventId, 'firstImage']);
        });
      }
    }
  );

  // Loading / Error
  if (loadingRegs) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }
  if (regsError) {
    return (
      <Box mt={4}>
        <Alert
          severity="error"
          action={<Button onClick={() => queryClient.invalidateQueries(['myRegistrations'])}>Retry</Button>}
        >
          Failed to load your registrations.
        </Alert>
      </Box>
    );
  }
  if (regs.length === 0) {
    return (
      <Box textAlign="center" mt={8}>
        <Typography>You haven’t registered for any events yet.</Typography>
        <Button component={Link} to="/events" variant="contained" sx={{ mt: 2 }}>
          Browse all events
        </Button>
      </Box>
    );
  }

  // Рендерим карточки
  return (
    <Box p={4}>
      <Typography variant="h4" gutterBottom>
        My Events
      </Typography>

      <Grid container spacing={2}>
        {regs.map((reg, idx) => {
          const eventQ = eventQueries[idx];
          const imgQ = imageQueries[idx];
          const loading = eventQ.isLoading || imgQ.isLoading;
          const error = eventQ.isError || imgQ.isError;

          // Если загрузка или ошибка одного из запросов — показываем заглушку
          if (loading || error) {
            return (
              <Grid item xs={12} sm={6} md={4} key={reg.id}>
                <Card>
                  <CardContent>
                    {loading ? (
                      <CircularProgress />
                    ) : (
                      <Alert severity="error">Error loading event</Alert>
                    )}
                  </CardContent>
                </Card>
              </Grid>
            );
          }

          const event = eventQ.data;
          const imgUrl = imgQ.data || PLACEHOLDER_IMAGE;

          return (
            <Grid item xs={12} sm={6} md={4} key={reg.id}>
              <Card>
                <CardActionArea component={Link} to={`/events/${event.id}`}>
                  <CardMedia
                    component="img"
                    height="140"
                    image={imgUrl}
                    alt={event.title}
                  />
                  <CardContent>
                    <Typography variant="h6">{event.title}</Typography>
                    <Typography variant="body2" color="textSecondary">
                      {new Date(event.date).toLocaleString()} — {event.venue}
                    </Typography>
                    <Typography variant="body2" color="textSecondary" sx={{ mt: 1 }}>
                      Registered on:{' '}
                      {new Date(reg.dateOfRegistration).toLocaleDateString()}
                    </Typography>
                  </CardContent>
                </CardActionArea>
                <Box display="flex" justifyContent="flex-end" p={2}>
                  <Button
                    variant="contained"
                    color="warning"
                    disabled={unregisterMutation.isLoading}
                    onClick={() => {
                      if (window.confirm('Are you sure you want to unregister?')) {
                        unregisterMutation.mutate({ registrationId: reg.id, eventId: event.id });
                      }
                    }}
                  >
                    Unregister
                  </Button>
                </Box>
              </Card>
            </Grid>
          );
        })}
      </Grid>
    </Box>
  );
}
