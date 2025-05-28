import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Box,
  Paper,
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
  useTheme,
  useMediaQuery,
} from '@mui/material';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getCategories, createCategory } from '../../api/categories';
import { createEvent } from '../../api/events';
import { uploadEventImage } from '../../api/images';

export default function CreateEventPage() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const theme = useTheme();
  const isSmall = useMediaQuery(theme.breakpoints.down('sm'));

  const [form, setForm] = useState({
    title: '',
    date: '',
    venue: '',
    categoryId: '',
    description: '',
    capacity: '',
  });
  const [files, setFiles] = useState([]);

  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');

  const { data: categories = [] } = useQuery({
    queryKey: ['categories'],
    queryFn: () => getCategories().then(res => res.data),
  });

  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newCategoryName, setNewCategoryName] = useState('');
  const createCatMutation = useMutation({
    mutationFn: name => createCategory({ name }).then(res => res.data),
    onSuccess: newId => {
      queryClient.invalidateQueries(['categories']);
      setForm(f => ({ ...f, categoryId: newId }));
      setNewCategoryName('');
      setIsModalOpen(false);
    },
  });

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleFileChange = e => {
    if (!e.target.files) return;
    setFiles(Array.from(e.target.files));
  };

  const handleSubmit = async e => {
    e.preventDefault();
    setLoading(true);
    setErrorMessage('');
    setSuccessMessage('');

    try {
      const { data } = await createEvent({
        title: form.title,
        date: form.date,
        venue: form.venue,
        categoryId: form.categoryId,
        description: form.description,
        capacity: Number(form.capacity),
      });
      const eventId = data.id;

      if (files.length) {
        await Promise.all(
          files.map(file => uploadEventImage(eventId, file))
        );
      }

      setSuccessMessage('Event and images uploaded successfully!');
      setTimeout(() => navigate('/admin/events', { replace: true }), 800);
    } catch (err) {
      setErrorMessage(err.message || 'An error occurred. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="sm" sx={{ my: 4 }}>
      <Paper sx={{ p: { xs: 2, sm: 4 }, boxShadow: 3 }}>
        <Typography variant={isSmall ? 'h6' : 'h4'} align="center" gutterBottom>
          Create New Event
        </Typography>

        {errorMessage && <Alert severity="error" sx={{ mb: 2 }}>{errorMessage}</Alert>}
        {successMessage && <Alert severity="success" sx={{ mb: 2 }}>{successMessage}</Alert>}

        <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
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
            InputLabelProps={{ shrink: true }}
            required
            fullWidth
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
              <MenuItem value=""><em>Select a category</em></MenuItem>
              {categories.map(cat => (
                <MenuItem key={cat.id} value={cat.id}>{cat.name}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <Button variant="outlined" onClick={() => setIsModalOpen(true)}>
            Add Category
          </Button>

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

          <Button variant="outlined" component="label">
            Select Images
            <input
              type="file"
              hidden
              multiple
              accept="image/*"
              onChange={handleFileChange}
            />
          </Button>
          {files.length > 0 && (
            <Typography variant="body2">{files.length} file(s) selected</Typography>
          )}

          <Box sx={{ mt: 2 }}>
            <Button
              type="submit"
              variant="contained"
              disabled={loading}
              startIcon={loading ? <CircularProgress size={20} /> : null}
              fullWidth
            >
              {loading ? 'Creating…' : 'Create Event'}
            </Button>
          </Box>
        </Box>

        <Dialog open={isModalOpen} onClose={() => setIsModalOpen(false)} fullWidth maxWidth="xs">
          <DialogTitle>Add New Category</DialogTitle>
          <DialogContent>
            <TextField
              autoFocus
              margin="dense"
              label="Category Name"
              value={newCategoryName}
              onChange={e => setNewCategoryName(e.target.value)}
              fullWidth
            />
          </DialogContent>
          <DialogActions>
            <Button onClick={() => setIsModalOpen(false)}>Cancel</Button>
            <Button
              onClick={() => createCatMutation.mutate(newCategoryName)}
              disabled={createCatMutation.isLoading}
              startIcon={createCatMutation.isLoading && <CircularProgress size={20} />}
            >
              {createCatMutation.isLoading ? 'Adding…' : 'Add'}
            </Button>
          </DialogActions>
        </Dialog>
      </Paper>
    </Container>
  );
}
