import React from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import {
  Container, Box, Typography, Button,
  CircularProgress, Alert, CardMedia,
  List, ListItem, ListItemText
} from '@mui/material';
import Slider from 'react-slick';

import { useAuth } from '../../context/AuthContext';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getEventById, deleteEvent, uploadEventImage
} from '../../api/events';
import {
  getParticipants, registerParticipant,
  unregisterParticipant, isRegistered as checkRegistration
} from '../../api/participants';
import { getEventImages } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';

export default function EventDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user, accessToken } = useAuth();
  const isAdmin = user?.roles?.includes('Admin') ?? false;
  const queryClient = useQueryClient();

  // 1) основной запрос события
  const {
    data: event,
    isLoading: loadingEvent,
    error: eventError
  } = useQuery({
    queryKey: ['event', id],
    queryFn: () => getEventById(id).then(res => res.data),
  });

  // 2) запрос списка картинок
  const {
    data: imageUrls = [],
    isLoading: loadingImages,
    error: imagesError
  } = useQuery({
    queryKey: ['eventImages', id],
    queryFn: () => getEventImages(id),
    enabled: !!event,
    staleTime: 5 * 60_000,
  });

  // 3) проверка, зарегистрирован ли текущий пользователь
  const {
    data: registration,
    isLoading: loadingRegistration,
    error: registrationError
  } = useQuery({
    queryKey: ['isRegistered', id],
    queryFn: () => checkRegistration(id).then(res => res.data),
    enabled: !!user && !isAdmin,
  });

  const registerMutation = useMutation({
    mutationFn: () => registerParticipant({ eventId: id }),
    onSuccess: () => {
      queryClient.invalidateQueries(['isRegistered', id]);
      queryClient.invalidateQueries(['participants', id]);
    },
  });

  const unregisterMutation = useMutation({
    mutationFn: () => unregisterParticipant(registration.participantId, id),
    onSuccess: () => {
      queryClient.invalidateQueries(['isRegistered', id]);
      queryClient.invalidateQueries(['participants', id]);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => deleteEvent(id),
    onSuccess: () => navigate('/events', { replace: true }),
  });

  const uploadMutation = useMutation({
    mutationFn: file => uploadEventImage(id, { file }),
    onSuccess: () => {
      queryClient.invalidateQueries(['eventImages', id]);
      queryClient.invalidateQueries(['event', id]);
    },
  });

  // загрузка...
  if (loadingEvent) return (
    <Box display="flex" justifyContent="center" mt={8}>
      <CircularProgress />
    </Box>
  );
  if (eventError) return (
    <Container maxWidth="sm" sx={{ mt: 4 }}>
      <Alert severity="error">Error loading event: {eventError.message}</Alert>
    </Container>
  );
  if (!event) return (
    <Box textAlign="center" mt={8}>
      <Typography>Event not found.</Typography>
      <Button component={Link} to="/events" sx={{ mt: 2 }}>
        Back to events
      </Button>
    </Box>
  );

  // готовим массив картинок для слайдера
  const images = !loadingImages && imageUrls.length
    ? imageUrls.map(url => ({ id: url, url }))
    : [{ id: 'placeholder', url: PLACEHOLDER_IMAGE }];

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        {event.title}
      </Typography>

      {/* Слайдер картинок */}
      <Box mb={3}>
        {images.length > 1 ? (
          <Slider dots infinite speed={500} slidesToShow={1} slidesToScroll={1}>
            {images.map(img => (
              <CardMedia
                key={img.id}
                component="img"
                image={img.url}
                alt={event.title}
                height="400"
                sx={{ borderRadius: 1, mb: 2 }}
              />
            ))}
          </Slider>
        ) : (
          <CardMedia
            component="img"
            image={images[0].url}
            alt={event.title}
            height="400"
            sx={{ borderRadius: 1 }}
          />
        )}
      </Box>

      {/* Описание */}
      <Typography variant="subtitle1" color="textSecondary" gutterBottom>
        {new Date(event.date).toLocaleString()} — {event.venue}
      </Typography>
      <Typography variant="body1" paragraph>
        {event.description}
      </Typography>

      {/* Регистрация */}
      {!isAdmin && accessToken && (
        <Box mb={3}>
          {loadingRegistration ? (
            <CircularProgress size={24} />
          ) : registrationError ? (
            <Alert severity="error">Error: {registrationError.message}</Alert>
          ) : registration.isRegistered ? (
            <Button
              variant="contained"
              color="warning"
              onClick={() => unregisterMutation.mutate()}
              disabled={unregisterMutation.isLoading}
            >
              Cancel Registration
            </Button>
          ) : (
            <Button
              variant="contained"
              color="primary"
              onClick={() => registerMutation.mutate()}
              disabled={registerMutation.isLoading}
            >
              Register
            </Button>
          )}
        </Box>
      )}

      {/* Список участников (доступен всем) */}
      <Box mb={3}>
        <Typography variant="h6">Participants</Typography>
        <ParticipantsList eventId={id} />
      </Box>

      {/* Админ-панель */}
      {isAdmin && (
        <Box mb={3}>
          <Box display="flex" gap={1} mb={2}>
            <Button
              variant="outlined"
              onClick={() => navigate(`/admin/events/edit/${id}`)}
            >
              Edit
            </Button>
            <Button
              variant="outlined"
              color="error"
              onClick={() => deleteMutation.mutate()}
              disabled={deleteMutation.isLoading}
            >
              Delete
            </Button>
            <Button
              variant="outlined"
              component="label"
              disabled={uploadMutation.isLoading}
            >
              {uploadMutation.isLoading ? 'Uploading…' : 'Upload Photo'}
              <input
                type="file"
                hidden
                accept="image/*"
                onChange={e => uploadMutation.mutate(e.target.files[0])}
              />
            </Button>
          </Box>
        </Box>
      )}

      <Button component={Link} to="/events" variant="text">
        Back to events
      </Button>
    </Container>
  );
}

// Вынесли список участников в отдельный компонент для чистоты кода
function ParticipantsList({ eventId }) {
  const { data: participants = [], isLoading } = useQuery({
    queryKey: ['participants', eventId],
    queryFn: () => getParticipants(eventId).then(res => res.data),
  });

  if (isLoading) return <CircularProgress size={24} />;
  if (!participants.length) return <Typography>No participants yet.</Typography>;

  return (
    <List>
      {participants.map(p => (
        <ListItem key={p.id} divider>
          <ListItemText primary={p.fullName || p.email} secondary={p.email} />
        </ListItem>
      ))}
    </List>
  );
}
