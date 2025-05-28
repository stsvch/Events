import React, { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Typography,
  TextField,
  IconButton,
  Collapse,
  Paper,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Switch,
  FormControlLabel,
  Button,
  CircularProgress,
  Alert
} from '@mui/material';
import FilterListIcon from '@mui/icons-material/FilterList';
import EventsGrid from './EventsGrid';
import { useQuery, useQueries } from '@tanstack/react-query';
import { getEvents } from '../../api/events';
import { getCategories } from '../../api/categories';
import { getFirstEventImage } from '../../api/images';

const PLACEHOLDER = '/images/placeholder.png';
const PAGE_SIZE = 4;

export default function EventsListPage() {
  const [page, setPage] = useState(1);
  const [combineMode, setCombineMode] = useState('And'); 
  const [searchTerm, setSearchTerm] = useState('');
  const [debounced, setDebounced] = useState('');
  const [venueFilter, setVenueFilter] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [showFilters, setShowFilters] = useState(false);

  // дебаунс для поиска
  useEffect(() => {
    const t = setTimeout(() => {
      setDebounced(searchTerm.trim());
      setPage(1);
    }, 500);
    return () => clearTimeout(t);
  }, [searchTerm]);

  // категории
  const { data: categories = [], isLoading: loadingCats, error: catsError } = useQuery({
    queryKey: ['categories'],
    queryFn: () => getCategories().then(res => res.data),
  });

  // параметры запроса событий
  const fetchParams = useMemo(() => ({
    pageNumber: page,
    pageSize: PAGE_SIZE,
    title: debounced || undefined,
    venue: venueFilter || undefined,
    startDate: startDate || undefined,
    endDate: endDate || undefined,
    categoryId: categoryId || undefined,
    combineMode,
  }), [page, debounced, venueFilter, startDate, endDate, categoryId, combineMode]);

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

  // тянем первые картинки
  const firstImageQueries = useQueries({
    queries: events.map(evt => ({
      queryKey: ['event', evt.id, 'firstImage'],
      queryFn: () => getFirstEventImage(evt.id),
      enabled: !!events.length,
      retry: false,
      onError: err => {
        if (err.response?.status !== 404) console.error(err);
      }
    }))
  });
  const firstImageMap = useMemo(() => {
    const map = {};
    events.forEach((evt, idx) => {
      const q = firstImageQueries[idx];
      if (q.isSuccess) map[evt.id] = q.data;
      else if (q.isError && q.error?.response?.status === 404) map[evt.id] = PLACEHOLDER;
      else map[evt.id] = null;
    });
    return map;
  }, [events, firstImageQueries]);

  // ручки
  const handleClear = () => {
    setCategoryId('');
    setVenueFilter('');
    setStartDate('');
    setEndDate('');
    setCombineMode('And');
  };
  const handleApply = () => {
    setPage(1);
    setShowFilters(false);
  };

  return (
    <Box p={4}>
      <Typography variant="h4" mb={3}>Events</Typography>

      {/* Строка поиска + кнопка фильтров */}
      <Box display="flex" alignItems="center" gap={1} mb={2}>
        <TextField
          fullWidth
          variant="outlined"
          placeholder="Search events by title..."
          value={searchTerm}
          onChange={e => setSearchTerm(e.target.value)}
        />
        <IconButton
          color={showFilters ? 'primary' : 'default'}
          onClick={() => setShowFilters(f => !f)}
        >
          <FilterListIcon />
        </IconButton>
      </Box>

      {/* Панель фильтров */}
      <Collapse in={showFilters}>
        <Paper variant="outlined" sx={{ p: 2, mb: 4 }}>
          <Box display="flex" flexWrap="wrap" gap={2}>
            <FormControl sx={{ minWidth: 160 }}>
              <InputLabel>Category</InputLabel>
              <Select
                value={categoryId}
                label="Category"
                onChange={e => setCategoryId(e.target.value)}
              >
                <MenuItem value="">All Categories</MenuItem>
                {categories.map(cat => (
                  <MenuItem key={cat.id} value={cat.id}>{cat.name}</MenuItem>
                ))}
              </Select>
            </FormControl>

            <FormControlLabel
              control={
                <Switch
                  checked={combineMode === 'And'}
                  onChange={e => setCombineMode(e.target.checked ? 'And' : 'Or')}
                />
              }
              label={combineMode === 'And' ? 'Match ALL (AND)' : 'Match ANY (OR)'}
            />

            <TextField
              label="Start Date"
              type="date"
              InputLabelProps={{ shrink: true }}
              value={startDate}
              onChange={e => setStartDate(e.target.value)}
            />
            <TextField
              label="End Date"
              type="date"
              InputLabelProps={{ shrink: true }}
              value={endDate}
              onChange={e => setEndDate(e.target.value)}
            />
            <TextField
              label="Venue"
              value={venueFilter}
              onChange={e => setVenueFilter(e.target.value)}
            />
          </Box>

          <Box display="flex" justifyContent="space-between" mt={3}>
            <Button color="secondary" onClick={handleClear}>
              Clear the filters
            </Button>
            <Box>
              <Button onClick={() => setShowFilters(false)} sx={{ mr: 1 }}>
                Cancel
              </Button>
              <Button variant="contained" onClick={handleApply}>
                Apply
              </Button>
            </Box>
          </Box>
        </Paper>
      </Collapse>

      {/* Список или загрузка/ошибка */}
      {loadingEvents ? (
        <Box display="flex" justifyContent="center" mt={4}>
          <CircularProgress />
        </Box>
      ) : eventsError ? (
        <Alert
          severity="error"
          action={
            <Button color="inherit" size="small" onClick={refetchEvents}>
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
        <EventsGrid events={events} imageMap={firstImageMap} />
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
