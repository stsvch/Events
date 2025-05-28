import React, { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Typography,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  List,
  ListItem,
  ListItemAvatar,
  Avatar,
  ListItemText,
  Button,
  CircularProgress,
  Alert
} from '@mui/material';
import EventsGrid from './EventsGrid';
import { Link } from 'react-router-dom';
import { useQuery, useQueries } from '@tanstack/react-query';
import { getEvents } from '../../api/events';
import { getCategories } from '../../api/categories';
import { getFirstEventImage } from '../../api/images';

const PLACEHOLDER = '/images/placeholder.png';
const PAGE_SIZE = 10;

export default function EventsListPage() {
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState('');
  const [debounced, setDebounced] = useState('');
  const [venueFilter, setVenueFilter] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [categoryId, setCategoryId] = useState('');

  useEffect(() => {
    const timeout = setTimeout(() => {
      setDebounced(searchTerm.trim());
      setPage(1);
    }, 500);
    return () => clearTimeout(timeout);
  }, [searchTerm]);

  // Загрузка категорий (объектная форма)
  const { data: categories = [], isLoading: loadingCats, error: catsError } = useQuery({
    queryKey: ['categories'],
    queryFn: () => getCategories().then(res => res.data),
  });

  // Параметры для запроса событий
  const fetchParams = useMemo(() => ({
    pageNumber: page,
    pageSize: PAGE_SIZE,
    title: debounced || undefined,
    venue: venueFilter || undefined,
    startDate: startDate || undefined,
    endDate: endDate || undefined,
    categoryId: categoryId || undefined,
  }), [page, debounced, venueFilter, startDate, endDate, categoryId]);

  const {
    data: eventsData,
    isLoading: loadingEvents,
    error: eventsError,
    refetch: refetchEvents
  } = useQuery({
    queryKey: ['events', fetchParams],
    queryFn: () => getEvents(fetchParams).then(res => res.data),
    keepPreviousData: true,
  });

  const events = eventsData?.items || [];
  const totalCount = eventsData?.totalCount || 0;
  const totalPages = Math.ceil(totalCount / PAGE_SIZE);

  // ————————————————————————————————————————————————
  // Параллельно запрашиваем первую картинку для каждого события
  const firstImageQueries = useQueries({
    queries: events.map(evt => ({
      queryKey: ['event', evt.id, 'firstImage'],
      queryFn: () => getFirstEventImage(evt.id),
      // Не делать запрос, пока не выгрузились сами события:
      enabled: !!events.length,
      // Пока нет данных — возвращаем заглушку
      placeholderData: PLACEHOLDER,
      // Если 404 (нет картинок) — будем считать, что картинки нет
      onError: (err) => {
        if (err.response?.status !== 404) {
          console.error(`Error fetching first image for ${evt.id}`, err);
        }
      }
    }))
  });

  // Собираем мапинг eventId → imageUrl или PLACEHOLDER
  const firstImageMap = useMemo(() => {
    const map = {};
    events.forEach((evt, idx) => {
      const q = firstImageQueries[idx];
      map[evt.id] = q.data || PLACEHOLDER;
    });
    return map;
  }, [events, firstImageQueries]);

  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>Events</Typography>

      {/* Поиск */}
      <Box mb={2}>
        <TextField
          fullWidth
          placeholder="Search events by title..."
          value={searchTerm}
          onChange={e => setSearchTerm(e.target.value)}
        />
      </Box>

      {/* Фильтры */}
      <Box display="flex" flexWrap="wrap" gap={2} mb={4}>
        <FormControl sx={{ minWidth: 160 }}>
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

        <TextField
          label="Start Date"
          type="date"
          InputLabelProps={{ shrink: true }}
          value={startDate}
          onChange={e => { setStartDate(e.target.value); setPage(1); }}
        />
        <TextField
          label="End Date"
          type="date"
          InputLabelProps={{ shrink: true }}
          value={endDate}
          onChange={e => { setEndDate(e.target.value); setPage(1); }}
        />
        <TextField
          label="Venue"
          value={venueFilter}
          onChange={e => { setVenueFilter(e.target.value); setPage(1); }}
        />
      </Box>

      {/* Список */}
      {loadingEvents ? (
        <Box display="flex" justifyContent="center" mt={4}>
          <CircularProgress />
        </Box>
      ) : eventsError ? (
        <Alert
          severity="error"
          action={
            <Button color="inherit" size="small" onClick={() => refetchEvents()}>
              Retry
            </Button>
          }
        >
          {eventsError.message || 'Failed to load events'}
        </Alert>
      ) : events.length === 0 ? (
        <Typography align="center" color="textSecondary">
          No events found with current filters.
        </Typography>
      ) : (
        <EventsGrid events={events} />
      )}

      {/* Пагинация */}
      {!loadingEvents && !eventsError && events.length > 0 && (
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
