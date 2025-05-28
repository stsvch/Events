// src/pages/Events/EventDetailPage.jsx

import React from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Button,
  CircularProgress,
  Alert,
  CardMedia,
  List,
  ListItem,
  ListItemText,
} from '@mui/material';
import EventIcon from '@mui/icons-material/Event';
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import LocationOnIcon from '@mui/icons-material/LocationOn';
import Slider from 'react-slick';
import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';

import { useAuth } from '../../context/AuthContext';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getEventById, deleteEvent } from '../../api/events';
import {
  getParticipants,
  unregisterParticipant,
  isRegistered as checkRegistration,
} from '../../api/participants';
import { getEventImages, uploadEventImage } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';

export default function EventDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user, accessToken } = useAuth();
  const isAdmin = user?.roles?.includes('Admin') ?? false;
  const queryClient = useQueryClient();

  // 1) Fetch event details
  const {
    data: event,
    isLoading: loadingEvent,
    error: eventError,
  } = useQuery({
    queryKey: ['event', id],
    queryFn: () => getEventById(id).then(res => res.data),
  });

  // 2) Fetch event images
  const {
    data: imageList = [],
    isLoading: loadingImages,
    error: imagesError,
  } = useQuery({
    queryKey: ['eventImages', id],
    queryFn: () => getEventImages(id),
    enabled: !!event,
    staleTime: 5 * 60_000,
  });

  // 3) Check current user's registration status
  const {
    data: registration,
    isLoading: loadingRegistration,
    error: registrationError,
  } = useQuery({
    queryKey: ['isRegistered', id],
    queryFn: () => checkRegistration(id).then(res => res.data),
    enabled: !!user && !isAdmin,
  });

  // Mutations
  const unregisterMutation = useMutation({
    mutationFn: () => unregisterParticipant(id),
    onSuccess: () => {
      queryClient.invalidateQueries(['isRegistered', id]);
      queryClient.invalidateQueries(['participants', id]);
      queryClient.invalidateQueries(['event', id]);
    },
  });

  const deleteMutation = useMutation({
    mutationFn: () => deleteEvent(id),
    onSuccess: () => navigate('/events', { replace: true }),
  });

  const uploadMutation = useMutation({
    mutationFn: file => uploadEventImage(id, file),
    onSuccess: () => {
      queryClient.invalidateQueries(['eventImages', id]);
      queryClient.invalidateQueries(['event', id]);
    },
  });

  // Show loading / error states
  if (loadingEvent) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }
  if (eventError) {
    return (
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Alert severity="error">Error loading event: {eventError.message}</Alert>
      </Container>
    );
  }
  if (!event) {
    return (
      <Box textAlign="center" mt={8}>
        <Typography>Event not found.</Typography>
        <Button component={Link} to="/events" sx={{ mt: 2 }}>
          Back to events
        </Button>
      </Box>
    );
  }

  // Prepare images for slider
  const images = !loadingImages && imageList.length
    ? imageList.map(img => ({ id: img.id, url: img.url }))
    : [{ id: 'placeholder', url: PLACEHOLDER_IMAGE }];

  const isFull = event.participantCount >= event.capacity;

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      {/* Event Title */}
      <Typography variant="h4" gutterBottom>
        {event.title}
      </Typography>

      {/* Image Slider */}
      <Box mb={3}>
        {images.length > 1 ? (
          <Slider
            dots
            infinite
            speed={500}
            slidesToShow={1}
            slidesToScroll={1}
          >
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

      {/* Event Details */}
      <Box mb={2}>
        <Box display="flex" alignItems="center" gap={1}>
          <EventIcon fontSize="small" />
          <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
            {new Date(event.date).toLocaleDateString(undefined, {
              year: 'numeric',
              month: '2-digit',
              day: '2-digit',
            })}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" gap={1} mt={1}>
          <AccessTimeIcon fontSize="small" />
          <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
            {new Date(event.date).toLocaleTimeString(undefined, {
              hour: '2-digit',
              minute: '2-digit',
            })}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" gap={1} mt={1}>
          <LocationOnIcon fontSize="small" />
          <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
            {event.venue}
          </Typography>
        </Box>
        <Box display="flex" alignItems="center" gap={4} mt={1}>
          <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
            Capacity: {event.capacity}
          </Typography>
          <Typography variant="subtitle1" sx={{ fontWeight: 500 }}>
            Registered: {event.participantCount}
          </Typography>
        </Box>
      </Box>

      {/* Description */}
      <Typography variant="body1" paragraph>
        {event.description}
      </Typography>

      {/* Registration Controls */}
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
            <>
              <Button
                variant="contained"
                color="primary"
                onClick={() => navigate(`/participants/register?eventId=${id}`)}
                disabled={isFull}
                title={isFull ? 'This event is sold out' : 'Register for this event'}
              >
                {isFull ? 'Sold Out' : 'Register'}
              </Button>
              {isFull && (
                <Typography color="error" sx={{ mt: 1 }}>
                  Sorry, this event is sold out.
                </Typography>
              )}
            </>
          )}
        </Box>
      )}

      {/* Participants List */}
      <Box mb={3}>
        <Typography variant="h6">Participants</Typography>
        <ParticipantsList eventId={id} />
      </Box>

      {/* Admin Panel */}
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

      {/* Back to Events */}
      <Button component={Link} to="/events" variant="text">
        Back to Events
      </Button>
    </Container>
  );
}

function ParticipantsList({ eventId }) {
  const { data: participants = [], isLoading } = useQuery({
    queryKey: ['participants', eventId],
    queryFn: () => getParticipants(eventId).then(res => res.data),
  });

  if (isLoading) return <CircularProgress size={24} />;
  if (!participants.length)
    return <Typography color="textSecondary">No participants yet.</Typography>;

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
