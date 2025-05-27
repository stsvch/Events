// src/pages/Events/EditEventPage.jsx
import React, { useState, useEffect } from 'react';
import { useParams, useHistory } from 'react-router-dom';
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
  Grid,
} from '@mui/material';
import { getEventById, updateEvent } from '../../api/events';
import { getCategories } from '../../api/categories';

export default function EditEventPage() {
  const { id } = useParams();
  const history = useHistory();

  const [form, setForm] = useState({
    title: '',
    date: '',
    venue: '',
    categoryId: '',
    description: '',
    capacity: '',
  });
  const [categories, setCategories] = useState([]);

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');

  // Load event and categories on mount
  useEffect(() => {
    (async () => {
      setLoading(true);
      setError('');
      try {
        const [{ data: evt }, { data: cats }] = await Promise.all([
          getEventById(id),
          getCategories(),
        ]);
        setForm({
          title: evt.title,
          // slice to fit <input type="datetime-local" />
          date: evt.date.slice(0, 16),
          venue: evt.venue,
          categoryId: evt.categoryId,
          description: evt.description,
          capacity: String(evt.capacity),
        });
        setCategories(cats);
      } catch (err) {
        setError(err.message || 'Failed to load data');
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setSaving(true);
    setError('');
    try {
      const payload = {
        ...form,
        capacity: parseInt(form.capacity, 10),
      };
      await updateEvent(id, payload);
      history.push('/admin/events');
    } catch (err) {
      setError(err.message || 'Failed to update event');
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" mt={8}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="sm" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h5" gutterBottom>
        Edit Event
      </Typography>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
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
            disabled={saving}
            startIcon={saving && <CircularProgress size={20} />}
          >
            {saving ? 'Saving…' : 'Save Changes'}
          </Button>
          <Button
            variant="outlined"
            onClick={() => history.push('/admin/events')}
          >
            Cancel
          </Button>
        </Box>
      </Box>
    </Container>
  );
}
