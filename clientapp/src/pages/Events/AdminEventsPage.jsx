// src/pages/Events/AdminEventsPage.jsx
import React, { useState, useEffect, useCallback } from 'react';
import { useHistory, Link } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Button,
  Grid,
  Card,
  CardMedia,
  CardContent,
  CardActions,
  CircularProgress,
  Alert,
} from '@mui/material';
import { getEvents, deleteEvent, uploadEventImage } from '../../api/events';

export default function AdminEventsPage() {
  const history = useHistory();

  // State
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // Pagination
  const [pageNumber, setPageNumber] = useState(1);
  const pageSize = 10;
  const [totalPages, setTotalPages] = useState(1);

  // Track which event is uploading
  const [uploadingId, setUploadingId] = useState(null);

  // Fetch events
  const fetchEvents = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const { data } = await getEvents({ pageNumber, pageSize });
      setEvents(data.items);
      setTotalPages(Math.ceil(data.totalCount / pageSize));
    } catch (err) {
      setError(err.message || 'Failed to load events');
    } finally {
      setLoading(false);
    }
  }, [pageNumber]);

  useEffect(() => {
    fetchEvents();
  }, [fetchEvents]);

  // Delete handler
  const handleDelete = async id => {
    if (!window.confirm('Are you sure you want to delete this event?')) return;
    try {
      await deleteEvent(id);
      fetchEvents();
    } catch (err) {
      alert(err.message || 'Delete failed');
    }
  };

  // Image upload handler
  const handleImageUpload = async (id, file) => {
    if (!file) return;
    setUploadingId(id);
    try {
      await uploadEventImage(id, file);
      fetchEvents();
    } catch {
      alert('Image upload failed');
    } finally {
      setUploadingId(null);
    }
  };

  const prevPage = () => setPageNumber(n => Math.max(1, n - 1));
  const nextPage = () => setPageNumber(n => Math.min(totalPages, n + 1));

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Admin: Manage Events</Typography>
        <Button
          variant="contained"
          color="success"
          onClick={() => history.push('/admin/events/create')}
        >
          Create New Event
        </Button>
      </Box>

      {/* Loading */}
      {loading && (
        <Box display="flex" justifyContent="center" mt={4}>
          <CircularProgress />
        </Box>
      )}

      {/* Error */}
      {error && (
        <Alert
          severity="error"
          action={
            <Button color="inherit" size="small" onClick={fetchEvents}>
              Retry
            </Button>
          }
          sx={{ mb: 3 }}
        >
          {error}
        </Alert>
      )}

      {/* Empty State */}
      {!loading && !error && events.length === 0 && (
        <Typography align="center" color="textSecondary">
          No events found.
        </Typography>
      )}

      {/* Event Grid */}
      {!loading && !error && events.length > 0 && (
        <Grid container spacing={3}>
          {events.map(evt => (
            <Grid item xs={12} sm={6} md={4} key={evt.id}>
              <Card>
                {evt.imageUrl && (
                  <CardMedia
                    component="img"
                    height="140"
                    image={evt.imageUrl}
                    alt={evt.title}
                  />
                )}
                <CardContent>
                  <Typography variant="h6">{evt.title}</Typography>
                  <Typography variant="body2" color="textSecondary">
                    {new Date(evt.date).toLocaleString()} — {evt.venue}
                  </Typography>
                </CardContent>
                <CardActions>
                  <Button
                    size="small"
                    onClick={() => history.push(`/admin/events/edit/${evt.id}`)}
                  >
                    Edit
                  </Button>
                  <Button
                    size="small"
                    color="error"
                    onClick={() => handleDelete(evt.id)}
                  >
                    Delete
                  </Button>
                  <Button
                    size="small"
                    component="label"
                    disabled={uploadingId === evt.id}
                  >
                    {uploadingId === evt.id ? 'Uploading…' : 'Upload Image'}
                    <input
                      type="file"
                      hidden
                      accept="image/*"
                      onChange={e => handleImageUpload(evt.id, e.target.files[0])}
                    />
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {/* Pagination */}
      {!loading && !error && events.length > 0 && (
        <Box display="flex" justifyContent="space-between" alignItems="center" mt={4}>
          <Button variant="outlined" disabled={pageNumber === 1} onClick={prevPage}>
            Previous
          </Button>
          <Typography>Page {pageNumber} of {totalPages}</Typography>
          <Button
            variant="outlined"
            disabled={pageNumber === totalPages}
            onClick={nextPage}
          >
            Next
          </Button>
        </Box>
      )}
    </Container>
  );
}
