// src/pages/Events/EventDetailPage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useHistory, Link } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Button,
  CircularProgress,
  Alert,
  CardMedia,
  Grid,
} from '@mui/material';
import Slider from "react-slick";
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

import { getEventById, uploadEventImage, deleteEvent } from '../../api/events';
import {
  getParticipants,
  registerParticipant,
  unregisterParticipant,
} from '../../api/participants';
import { useAuth } from '../../hooks/useAuth';

export default function EventDetailPage() {
  const { id } = useParams();
  const history = useHistory();
  const { user, accessToken } = useAuth();

  const [event, setEvent] = useState(null);
  const [participants, setParticipants] = useState([]);
  const [isRegistered, setIsRegistered] = useState(false);
  const [uploading, setUploading] = useState(false);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  // Settings for react-slick carousel
  const carouselSettings = {
    dots: true,
    infinite: true,
    speed: 500,
    slidesToShow: 1,
    slidesToScroll: 1,
  };

  // Load event & participants
  useEffect(() => {
    const fetchData = async () => {
      setLoading(true);
      setError('');
      try {
        const [{ data: evt }, { data: parts }] = await Promise.all([
          getEventById(id),
          getParticipants(id),
        ]);
        setEvent(evt);
        setParticipants(parts);
        setIsRegistered(parts.some(p => p.email === user?.email));
      } catch (err) {
        setError(err.message || 'Failed to load event');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, [id, user]);

  // Handlers
  const handleRegister = async () => {
    try {
      await registerParticipant({ eventId: id });
      setIsRegistered(true);
      const { data: parts } = await getParticipants(id);
      setParticipants(parts);
    } catch (err) {
      alert(err.message || 'Registration failed');
    }
  };

  const handleUnregister = async () => {
    try {
      const me = participants.find(p => p.email === user.email);
      await unregisterParticipant(me.id, id);
      setIsRegistered(false);
      const { data: parts } = await getParticipants(id);
      setParticipants(parts);
    } catch (err) {
      alert(err.message || 'Unregistration failed');
    }
  };

  const handleDelete = async () => {
    if (!window.confirm('Are you sure you want to delete this event?')) return;
    try {
      await deleteEvent(id);
      history.push('/events');
    } catch (err) {
      alert(err.message || 'Delete failed');
    }
  };

  const handleImageUpload = async e => {
    const file = e.target.files[0];
    if (!file) return;

    setUploading(true);
    try {
      await uploadEventImage(id, file);
      // Refetch event to get updated images array
      const { data: refreshed } = await getEventById(id);
      setEvent(refreshed);
    } catch (err) {
      alert(err.response?.data || 'Image upload failed');
    } finally {
      setUploading(false);
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
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Alert severity="error">{error}</Alert>
      </Container>
    );
  }

  if (!event) {
    return (
      <Box textAlign="center" mt={8}>
        <Typography>Event not found.</Typography>
        <Button component={Link} to="/events" sx={{ mt: 2 }}>
          Back to Events
        </Button>
      </Box>
    );
  }

  const isAdmin = user?.roles?.includes('Admin');

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" gutterBottom>
        {event.title}
      </Typography>

      {/* Carousel of images */}
      {event.images?.length > 0 && (
        <Box mb={3}>
          <Slider {...carouselSettings}>
            {event.images.map(img => (
              <Box key={img.id} sx={{ px: 1 }}>
                <CardMedia
                  component="img"
                  image={img.url}
                  alt={event.title}
                  height="400"
                  sx={{ borderRadius: 1 }}
                />
              </Box>
            ))}
          </Slider>
        </Box>
      )}

      <Typography variant="subtitle1" color="textSecondary" gutterBottom>
        {new Date(event.date).toLocaleString()} — {event.venue}
      </Typography>

      <Typography variant="body1" paragraph>
        {event.description}
      </Typography>

      <Grid container spacing={2} mb={3}>
        <Grid item>
          <Typography variant="body2">
            <strong>Capacity:</strong> {event.capacity}
          </Typography>
        </Grid>
        <Grid item>
          <Typography variant="body2">
            <strong>Registered:</strong> {participants.length}
          </Typography>
        </Grid>
      </Grid>

      {/* User actions */}
      {accessToken && !isAdmin && (
        <Box mb={3}>
          {isRegistered ? (
            <Button
              variant="contained"
              color="warning"
              onClick={handleUnregister}
            >
              Unregister
            </Button>
          ) : (
            <Button
              variant="contained"
              color="primary"
              onClick={handleRegister}
            >
              Register
            </Button>
          )}
        </Box>
      )}

      {/* Admin controls */}
      {isAdmin && (
        <Box mb={3} display="flex" gap={1}>
          <Button
            variant="outlined"
            onClick={() => history.push(`/admin/events/edit/${id}`)}
          >
            Edit
          </Button>
          <Button variant="outlined" color="error" onClick={handleDelete}>
            Delete
          </Button>
          <Button variant="outlined" component="label" disabled={uploading}>
            {uploading ? 'Uploading…' : 'Upload Image'}
            <input
              type="file"
              hidden
              accept="image/*"
              onChange={handleImageUpload}
            />
          </Button>
        </Box>
      )}

      <Button component={Link} to="/events" variant="text">
        Back to Events
      </Button>
    </Container>
  );
}
