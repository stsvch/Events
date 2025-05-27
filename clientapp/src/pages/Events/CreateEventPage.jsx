import React, { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import { getCategories, createCategory } from '../../api/categories';
import { createEvent } from '../../api/events';

export default function CreateEventPage() {
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
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  // For category creation modal
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newCategoryName, setNewCategoryName] = useState('');
  const [catLoading, setCatLoading] = useState(false);
  const [catError, setCatError] = useState('');

  // Load categories on mount
  useEffect(() => {
    fetchCategories();
  }, []);

  const fetchCategories = async () => {
    try {
      const { data } = await getCategories();
      setCategories(data);
    } catch {
      // ignore
    }
  };

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setLoading(true);
    setError('');
    try {
      const payload = {
        ...form,
        capacity: parseInt(form.capacity, 10),
      };
      await createEvent(payload);
      history.push('/admin/events');
    } catch (err) {
      setError(err.message || 'Failed to create event');
    } finally {
      setLoading(false);
    }
  };

  // Category modal handlers
  const openModal = () => {
    setNewCategoryName('');
    setCatError('');
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
  };

  const handleCreateCategory = async () => {
    if (!newCategoryName.trim()) {
      setCatError('Name is required');
      return;
    }
    setCatLoading(true);
    setCatError('');
    try {
      const { data: newId } = await createCategory({ name: newCategoryName });
      // refresh categories
      await fetchCategories();
      setForm(f => ({ ...f, categoryId: newId }));
      setIsModalOpen(false);
    } catch (err) {
      setCatError(err.message || 'Failed to create category');
    } finally {
      setCatLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h5" gutterBottom>
        Create New Event
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

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
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
          <Button variant="outlined" onClick={openModal}>
            Add
          </Button>
        </Box>

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
            disabled={loading}
            startIcon={loading && <CircularProgress size={20} />}
          >
            {loading ? 'Creating…' : 'Create Event'}
          </Button>
          <Button
            variant="outlined"
            onClick={() => history.push('/admin/events')}
          >
            Cancel
          </Button>
        </Box>
      </Box>

      {/* Category creation modal */}
      <Dialog open={isModalOpen} onClose={closeModal}>
        <DialogTitle>Create New Category</DialogTitle>
        <DialogContent>
          {catError && (
            <Alert severity="error" sx={{ mb: 2 }}>
              {catError}
            </Alert>
          )}
          <TextField
            autoFocus
            margin="dense"
            label="Category Name"
            fullWidth
            value={newCategoryName}
            onChange={e => setNewCategoryName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={closeModal} disabled={catLoading}>
            Cancel
          </Button>
          <Button onClick={handleCreateCategory} disabled={catLoading}>
            {catLoading ? <CircularProgress size={20} /> : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}
