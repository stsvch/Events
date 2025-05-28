// src/pages/Events/AdminEventsPage.jsx
import React, { useState, useMemo } from 'react';
import { useNavigate, Link } from 'react-router-dom';
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
import { useQuery, useMutation, useQueries, useQueryClient } from '@tanstack/react-query';
import { getEvents, deleteEvent, uploadEventImage } from '../../api/events';
import { getFirstEventImage } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';
const PAGE_SIZE = 10;

export default function AdminEventsPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [pageNumber, setPageNumber] = useState(1);

  // 1) Загрузка страниц событий
  const {
    data: eventsPage,
    isLoading: loadingEvents,
    isError: eventsError,
    error: eventsErrorObj,
  } = useQuery({
    queryKey: ['adminEvents', pageNumber],
    queryFn:   () => getEvents({ pageNumber, pageSize: PAGE_SIZE }).then(res => res.data),
    keepPreviousData: true,
  });


  const events = eventsPage?.items || [];
  const totalPages = useMemo(
    () => Math.ceil((eventsPage?.totalCount || 0) / PAGE_SIZE),
    [eventsPage]
  );

  // 2) Параллельная загрузка первой картинки для каждого события
  const imageQueries = useQueries({
    queries: events.map(evt => ({
      queryKey: ['event', evt.id, 'firstImage'],
      queryFn: () => getFirstEventImage(evt.id),
      enabled: !!events.length,
      placeholderData: PLACEHOLDER_IMAGE,
      onError: err => {
        // игнорируем 404
        if (err.response?.status !== 404) console.error(err);
      },
    })),
  });

  // 3) Мутации
  const deleteMutation = useMutation({
    mutationFn: id => deleteEvent(id),
    onSuccess:  () => queryClient.invalidateQueries(['adminEvents', pageNumber]),
  });

  const uploadMutation = useMutation({
    mutationFn: ({ eventId, file }) => uploadEventImage(eventId, { file }),
    onSuccess:  (_, { eventId }) => {
      queryClient.invalidateQueries(['adminEvents', pageNumber]);
      queryClient.invalidateQueries(['event', eventId, 'firstImage']);
    },
  });


  // Пагинация
  const prevPage = () => setPageNumber(n => Math.max(1, n - 1));
  const nextPage = () => setPageNumber(n => Math.min(totalPages, n + 1));

  // UI
  if (loadingEvents) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }
  if (eventsError) {
    return (
      <Alert
        severity="error"
        action={<Button onClick={() => queryClient.invalidateQueries(['adminEvents', pageNumber])}>Retry</Button>}
        sx={{ mb: 3 }}
      >
        {eventsErrorObj.message || 'Failed to load events'}
      </Alert>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Admin: Manage Events</Typography>
        <Button
          variant="contained"
          color="success"
          onClick={() => navigate('/admin/events/create')}
        >
          Create New Event
        </Button>
      </Box>

      {/* Empty State */}
      {!loadingEvents && events.length === 0 && (
        <Typography align="center" color="textSecondary">
          No events found.
        </Typography>
      )}

      {/* Grid */}
      <Grid container spacing={3}>
        {events.map((evt, idx) => {
          const imgQ = imageQueries[idx];
          const imgUrl = imgQ.data;
          const isUploading = uploadMutation.isLoading && uploadMutation.variables?.eventId === evt.id;

          return (
            <Grid item xs={12} sm={6} md={4} key={evt.id}>
              <Card>
                <CardMedia
                  component="img"
                  height="140"
                  image={imgUrl}
                  alt={evt.title}
                />
                <CardContent>
                  <Typography variant="h6">{evt.title}</Typography>
                  <Typography variant="body2" color="textSecondary">
                    {new Date(evt.date).toLocaleString()} — {evt.venue}
                  </Typography>
                </CardContent>
                <CardActions>
                  <Button
                    size="small"
                    component={Link}
                    to={`/admin/events/edit/${evt.id}`}
                  >
                    Edit
                  </Button>
                  <Button
                    size="small"
                    color="error"
                    onClick={() => {
                      if (window.confirm('Are you sure you want to delete this event?')) {
                        deleteMutation.mutate(evt.id);
                      }
                    }}
                    disabled={deleteMutation.isLoading}
                  >
                    Delete
                  </Button>
                  <Button
                    size="small"
                    component="label"
                    disabled={isUploading}
                  >
                    {isUploading ? 'Uploading…' : 'Upload Image'}
                    <input
                      type="file"
                      hidden
                      accept="image/*"
                      onChange={e =>
                        uploadMutation.mutate({ eventId: evt.id, file: e.target.files[0] })
                      }
                    />
                  </Button>
                </CardActions>
              </Card>
            </Grid>
          );
        })}
      </Grid>

      {/* Pagination */}
      {events.length > 0 && (
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
