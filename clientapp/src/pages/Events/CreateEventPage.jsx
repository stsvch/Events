// src/pages/Events/CreateEventPage.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
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
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getCategories, createCategory } from '../../api/categories';
import { createEvent, uploadEventImage } from '../../api/events';

export default function CreateEventPage() {
  const navigate = useNavigate();
  const qc = useQueryClient();

  // form state
  const [form, setForm] = useState({
    title: '',
    date: '',
    venue: '',
    categoryId: '',
    description: '',
    capacity: '',
  });
  // files selected by user
  const [files, setFiles] = useState([]);
  // Urls typed by user
  const [imageUrls, setImageUrls] = useState([]);

  // category modal
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newCategoryName, setNewCategoryName] = useState('');

  // — React Query: load categories
  const {
    data: categories = [],
    isLoading: loadingCats,
    isError: catsError,
    error: catsErrorObj,
  } = useQuery({
    queryKey: ['categories'],
    queryFn:   () => getCategories().then(r => r.data),
  });

  // — Create category mutation
  const createCatM = useMutation({
    mutationFn: name => createCategory({ name }).then(r => r.data),
    onSuccess:  newId => {
      qc.invalidateQueries(['categories']);
      setForm(f => ({ ...f, categoryId: newId }));
      setIsModalOpen(false);
    },
  });

  // — Create event mutation
  const createEvtM = useMutation({
    mutationFn: payload => createEvent(payload),
    onSuccess:  async ({ data: newEventId }) => {
        // upload each file
        for (let file of files) {
          await uploadEventImage(newEventId, { file });
        }
        // if any raw URLs
        for (let url of imageUrls) {
          await uploadEventImage(newEventId, { url });
        }
        navigate('/admin/events', { replace: true });
      }
    }
  );

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleFileChange = e => {
    setFiles(Array.from(e.target.files));
  };

  const handleUrlChange = (e, idx) => {
    const arr = [...imageUrls];
    arr[idx] = e.target.value;
    setImageUrls(arr);
  };

  const addUrlField = () => setImageUrls(u => [...u, '']);
  const removeUrlField = i => setImageUrls(u => u.filter((_, idx) => idx!==i));

  const handleSubmit = e => {
    e.preventDefault();
    createEvtM.mutate({
      ...form,
      capacity: parseInt(form.capacity, 10),
      ImageUrls: imageUrls.filter(u => u.trim()),
    });
  };

  return (
    <Container maxWidth="sm" sx={{ mt:4, mb:4 }}>
      <Typography variant="h5" gutterBottom>Create New Event</Typography>

      {(catsError || createCatM.isError || createEvtM.isError) && (
        <Alert severity="error" sx={{ mb:2 }}>
          {catsErrorObj?.message || createCatM.error?.message || createEvtM.error?.message}
        </Alert>
      )}

      <Box component="form" onSubmit={handleSubmit} sx={{ display:'flex', flexDirection:'column', gap:2 }}>
        <TextField label="Title" name="title" value={form.title} onChange={handleChange} required fullWidth />
        <TextField
          label="Date & Time" name="date" type="datetime-local"
          value={form.date} onChange={handleChange}
          InputLabelProps={{ shrink:true }} required fullWidth
        />
        <TextField label="Venue" name="venue" value={form.venue} onChange={handleChange} required fullWidth />

        <Box sx={{ display:'flex', gap:1 }}>
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
          <Button variant="outlined" onClick={()=>setIsModalOpen(true)}>Add</Button>
        </Box>

        <TextField
          label="Description" name="description"
          value={form.description} onChange={handleChange}
          multiline rows={4} fullWidth
        />

        <TextField
          label="Capacity" name="capacity" type="number"
          inputProps={{ min:0 }}
          value={form.capacity} onChange={handleChange}
          required fullWidth
        />

        {/* File inputs */}
        <Box>
          <Typography variant="subtitle2">Upload Photos</Typography>
          <input
            type="file" multiple accept="image/*"
            onChange={handleFileChange}
          />
        </Box>

        {/* Raw URL inputs */}
        {imageUrls.map((url, i) => (
          <Box key={i} sx={{ display:'flex', gap:1, alignItems:'center' }}>
            <TextField
              label={`Image URL #${i+1}`} value={url}
              onChange={e => handleUrlChange(e,i)} fullWidth
            />
            <Button onClick={()=>removeUrlField(i)}>×</Button>
          </Box>
        ))}
        <Button variant="text" onClick={addUrlField}>Add Image URL</Button>

        <Box sx={{ display:'flex', justifyContent:'space-between', mt:2 }}>
          <Button
            type="submit"
            variant="contained"
            disabled={createEvtM.isLoading}
            startIcon={createEvtM.isLoading && <CircularProgress size={20} />}
          >
            {createEvtM.isLoading ? 'Creating…' : 'Create Event'}
          </Button>
          <Button variant="outlined" onClick={()=>navigate('/admin/events', { replace:true })}>
            Cancel
          </Button>
        </Box>
      </Box>

      {/* Create Category Modal */}
      <Dialog open={isModalOpen} onClose={()=>setIsModalOpen(false)}>
        <DialogTitle>Create New Category</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus margin="dense" label="Category Name"
            fullWidth value={newCategoryName}
            onChange={e=>setNewCategoryName(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={()=>setIsModalOpen(false)}>Cancel</Button>
          <Button
            onClick={()=>createCatM.mutate(newCategoryName)}
            disabled={createCatM.isLoading}
          >
            {createCatM.isLoading ? <CircularProgress size={20}/> : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}
