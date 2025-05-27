// src/pages/Events/EventsListPage.jsx
import React, { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Grid,
  Card,
  CardActionArea,
  CardContent,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { getEvents } from '../../api/events';
import { getCategories } from '../../api/categories';

export default function EventsListPage() {
  const [events, setEvents] = useState([]);
  const [categories, setCategories] = useState([]);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // pagination
  const [page, setPage] = useState(1);
  const pageSize = 10;
  const [totalPages, setTotalPages] = useState(1);

  // filters
  const [searchTerm, setSearchTerm] = useState('');
  const [debounced, setDebounced] = useState('');
  const [venueFilter, setVenueFilter] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [categoryId, setCategoryId] = useState('');

  // debounce searchTerm
  useEffect(() => {
    const h = setTimeout(() => {
      setDebounced(searchTerm.trim());
      setPage(1);
    }, 500);
    return () => clearTimeout(h);
  }, [searchTerm]);

  // load categories once
  useEffect(() => {
    (async () => {
      try {
        const { data } = await getCategories();
        setCategories(data);
      } catch {
        // ignore
      }
    })();
  }, []);

  const fetchEvents = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const params = {
        pageNumber: page,
        pageSize,
        venue: venueFilter || undefined,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
        categoryId: categoryId || undefined,
      };
      if (debounced) {
        params.venue = debounced;
      }
      const { data } = await getEvents(params);
      setEvents(data.items);
      setTotalPages(Math.ceil(data.totalCount / pageSize));
    } catch (err) {
      setError(err.message || 'Failed to load events');
    } finally {
      setLoading(false);
    }
  }, [page, debounced, venueFilter, startDate, endDate, categoryId]);

  useEffect(() => {
    fetchEvents();
  }, [fetchEvents]);

  const handleRetry = () => fetchEvents();

  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>Events</Typography>

      {/* Filters */}
      <Grid container spacing={2} mb={4}>
        <Grid item xs={12} sm={6} md={4}>
          <TextField
            label="Search by title or venue"
            fullWidth
            value={searchTerm}
            onChange={e => setSearchTerm(e.target.value)}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={4}>
          <FormControl fullWidth>
            <InputLabel>Category</InputLabel>
            <Select
              value={categoryId}
              label="Category"
              onChange={e => { setCategoryId(e.target.value); setPage(1); }}
            >
              <MenuItem value="">All Categories</MenuItem>
              {categories.map(cat => (
                <MenuItem key={cat.id} value={cat.id}>{cat.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <TextField
            label="Start Date"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={startDate}
            onChange={e => { setStartDate(e.target.value); setPage(1); }}
          />
        </Grid>
        <Grid item xs={12} sm={6} md={2}>
          <TextField
            label="End Date"
            type="date"
            fullWidth
            InputLabelProps={{ shrink: true }}
            value={endDate}
            onChange={e => { setEndDate(e.target.value); setPage(1); }}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            label="Venue"
            fullWidth
            value={venueFilter}
            onChange={e => { setVenueFilter(e.target.value); setPage(1); }}
          />
        </Grid>
      </Grid>

      {/* Loading / Error */}
      {loading && (
        <Box display="flex" justifyContent="center" mt={4}>
          <CircularProgress />
        </Box>
      )}
      {error && (
        <Alert severity="error" action={
          <Button color="inherit" size="small" onClick={handleRetry}>
            Retry
          </Button>
        }>
          {error}
        </Alert>
      )}

      {/* Empty State */}
      {!loading && !error && events.length === 0 && (
        <Typography align="center" color="textSecondary">
          No events found with the current filters.<br/>
          Try resetting filters or changing your search.
        </Typography>
      )}

      {/* Event List */}
      {!loading && !error && events.length > 0 && (
        <Grid container spacing={2}>
          {events.map(evt => (
            <Grid item xs={12} sm={6} md={4} key={evt.id}>
              <Card>
                <CardActionArea component={Link} to={`/events/${evt.id}`}>
                  <CardContent>
                    <Typography variant="h6">{evt.title}</Typography>
                    <Typography variant="body2" color="textSecondary">
                      {new Date(evt.date).toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                      {evt.venue}
                    </Typography>
                  </CardContent>
                </CardActionArea>
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      {/* Pagination */}
      {!loading && !error && events.length > 0 && (
        <Box display="flex" justifyContent="space-between" alignItems="center" mt={4}>
          <Button
            variant="outlined"
            disabled={page === 1}
            onClick={() => setPage(p => p - 1)}
          >
            Previous
          </Button>
          <Typography>Page {page} of {totalPages}</Typography>
          <Button
            variant="outlined"
            disabled={page === totalPages}
            onClick={() => setPage(p => p + 1)}
          >
            Next
          </Button>
        </Box>
      )}
    </Box>
  );
}
