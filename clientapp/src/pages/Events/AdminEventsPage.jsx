// src/pages/Events/AdminEventsPage.jsx
import React, { useState, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  Button,
  CircularProgress,
  Alert
} from '@mui/material';
import {
  useQuery,
  useMutation,
  useQueries,
  useQueryClient
} from '@tanstack/react-query';

import AdminEventsGrid from '../../components/AdminEventsGrid';
import { getEvents, deleteEvent } from '../../api/events';
import { getFirstEventImage, uploadEventImage } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';
const PAGE_SIZE = 4;

export default function AdminEventsPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [pageNumber, setPageNumber] = useState(1);

  const {
    data: eventsPage,
    isLoading: loadingEvents,
    isError: eventsError,
    error: eventsErrorObj,
  } = useQuery({
    queryKey: ['adminEvents', pageNumber],
    queryFn: () =>
      getEvents({ pageNumber, pageSize: PAGE_SIZE }).then(res => res.data),
    keepPreviousData: true,
  });

  const events = eventsPage?.items || [];
  const totalPages = useMemo(
    () => Math.ceil((eventsPage?.totalCount || 0) / PAGE_SIZE),
    [eventsPage]
  );

  const imageQueries = useQueries({
    queries: events.map(evt => ({
      queryKey: ['event', evt.id, 'firstImage'],
      queryFn: () => getFirstEventImage(evt.id),
      enabled: events.length > 0,
      retry: false,
      placeholderData: PLACEHOLDER_IMAGE,
      onError: err => {
        if (err.response?.status !== 404) console.error(err);
      },
    })),
  });

  const deleteMutation = useMutation({
    mutationFn: id => deleteEvent(id),
    onSuccess: () =>
      queryClient.invalidateQueries(['adminEvents', pageNumber]),
  });

  const uploadMutation = useMutation({
    mutationFn: ({ eventId, file }) => uploadEventImage(eventId, { file }),
    onSuccess: (_, { eventId }) => {
      queryClient.invalidateQueries(['adminEvents', pageNumber]);
      queryClient.invalidateQueries(['event', eventId, 'firstImage']);
    },
  });

  const prevPage = () => setPageNumber(n => Math.max(1, n - 1));
  const nextPage = () => setPageNumber(n => Math.min(totalPages, n + 1));

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
        action={
          <Button onClick={() => queryClient.invalidateQueries(['adminEvents', pageNumber])}>
            Retry
          </Button>
        }
        sx={{ mb: 3 }}
      >
        {eventsErrorObj?.message || 'Failed to load events'}
      </Alert>
    );
  }

  const imageMap = events.reduce((map, evt, idx) => {
    const q = imageQueries[idx];
    map[evt.id] = q.data || PLACEHOLDER_IMAGE;
    return map;
  }, {});

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
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

      {events.length === 0 ? (
        <Typography align="center" color="textSecondary">
          No events found.
        </Typography>
      ) : (
        <>
          <AdminEventsGrid
            events={events}
            imageMap={imageMap}
            deleteLoadingId={deleteMutation.isLoading ? deleteMutation.variables : null}
            uploadLoadingId={uploadMutation.isLoading ? uploadMutation.variables?.eventId : null}
            onDelete={id => {
              if (window.confirm('Are you sure you want to delete this event?')) {
                deleteMutation.mutate(id);
              }
            }}
            onUpload={(eventId, file) => uploadMutation.mutate({ eventId, file })}
          />

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
        </>
      )}
    </Container>
  );
}
