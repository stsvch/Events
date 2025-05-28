// src/pages/Events/EditEventPage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Typography,
  TextField,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Button,
  CircularProgress,
  Alert,
} from '@mui/material';
import { useQuery, useMutation } from '@tanstack/react-query';
import { getEventById, updateEvent } from '../../api/events';
import { getCategories } from '../../api/categories';

export default function EditEventPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  // Форму заведём в локальный стейт
  const [form, setForm] = useState({
    title: '',
    date: '',
    venue: '',
    categoryId: '',
    description: '',
    capacity: '',
  });

  // ————————————————————————————————————————————————
  // 1) Загрузка события
  const {
    data: eventResponse,
    isLoading: loadingEvent,
    isError: isEventError,
    error: eventError,
  } = useQuery(
    ['event', id],
    () => getEventById(id).then(res => res.data),
    {
      onSuccess: evt => {
        setForm({
          title: evt.title,
          date: evt.date.slice(0, 16),
          venue: evt.venue,
          categoryId: evt.categoryId,
          description: evt.description,
          capacity: String(evt.capacity),
        });
      },
      staleTime: Infinity,
      cacheTime: Infinity,
    }
  );

  // 2) Загрузка категорий
  const {
    data: categories = [],
    isLoading: loadingCats,
    isError: isCatsError,
    error: catsError,
  } = useQuery(['categories'], () => getCategories().then(res => res.data), {
    staleTime: 5 * 60_000,
  });

  // 3) Мутация для сохранения
  const saveMutation = useMutation(
    updated => updateEvent(id, updated),
    {
      onSuccess: () => {
        navigate('/admin/events', { replace: true });
      },
    }
  );

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveMutation.mutate({
      ...form,
      capacity: parseInt(form.capacity, 10),
    });
  };

  if (loadingEvent || loadingCats) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }

  if (isEventError) {
    return (
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Alert severity="error">Error loading event: {eventError.message}</Alert>
      </Container>
    );
  }

  if (isCatsError) {
    return (
      <Container maxWidth="sm" sx={{ mt: 4 }}>
        <Alert severity="error">Error loading categories: {catsError.message}</Alert>
      </Container>
    );
  }

  // ————————————————————————————————————————————————
  return (
    <Container maxWidth="sm" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h5" gutterBottom>
        Edit Event
      </Typography>

      {saveMutation.isError && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {saveMutation.error.message || 'Failed to update event'}
        </Alert>
      )}

      <Box
        component="form"
        onSubmit={handleSubmit}
        noValidate
        sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}
      >
        <TextField
          label="Title"
          name="title"
          value={form.title}
          onChange={handleChange}
          required
          fullWidth
        />

        <TextField
          label="Date & Time"
          name="date"
          type="datetime-local"
          value={form.date}
          onChange={handleChange}
          required
          fullWidth
          InputLabelProps={{ shrink: true }}
        />

        <TextField
          label="Venue"
          name="venue"
          value={form.venue}
          onChange={handleChange}
          required
          fullWidth
        />

        <FormControl fullWidth>
          <InputLabel>Category</InputLabel>
          <Select
            label="Category"
            name="categoryId"
            value={form.categoryId}
            onChange={handleChange}
            required
          >
            <MenuItem value="">
              <em>Select a category</em>
            </MenuItem>
            {categories.map(cat => (
              <MenuItem key={cat.id} value={cat.id}>
                {cat.name}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <TextField
          label="Description"
          name="description"
          value={form.description}
          onChange={handleChange}
          multiline
          rows={4}
          fullWidth
        />

        <TextField
          label="Capacity"
          name="capacity"
          type="number"
          inputProps={{ min: 0 }}
          value={form.capacity}
          onChange={handleChange}
          required
          fullWidth
        />

        <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 2 }}>
          <Button
            type="submit"
            variant="contained"
            disabled={saveMutation.isLoading}
            startIcon={saveMutation.isLoading && <CircularProgress size={20} />}
          >
            {saveMutation.isLoading ? 'Saving…' : 'Save Changes'}
          </Button>
          <Button
            variant="outlined"
            onClick={() => navigate('/admin/events', { replace: true })}
          >
            Cancel
          </Button>
        </Box>
      </Box>
    </Container>
  );
}
