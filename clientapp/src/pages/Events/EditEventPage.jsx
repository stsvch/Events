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
  Snackbar,
  CardMedia,
} from '@mui/material';
import Slider from 'react-slick';
import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getEventById, updateEvent } from '../../api/events';
import { getCategories } from '../../api/categories';
import { getEventImages, deleteEventImage } from '../../api/images';

const PLACEHOLDER_IMAGE = '/images/placeholder.png';

export default function EditEventPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [form, setForm] = useState({ id: '', title: '', date: '', venue: '', categoryId: '', description: '', capacity: '' });
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [currentSlide, setCurrentSlide] = useState(0);

  const {
    data: eventData,
    isLoading: loadingEvent,
    isError: isEventError,
    error: eventError,
  } = useQuery({ queryKey: ['event', id], queryFn: () => getEventById(id).then(res => res.data) });

  const {
    data: imagesList = [],
    isLoading: loadingImages,
    isError: isImagesError,
    error: imagesError,
  } = useQuery({ queryKey: ['eventImages', id], queryFn: () => getEventImages(id), enabled: !!eventData });

  const {
    data: categories = [],
    isLoading: loadingCats,
    isError: isCatsError,
    error: catsError,
  } = useQuery({ queryKey: ['categories'], queryFn: () => getCategories().then(res => res.data) });

  useEffect(() => {
    if (!eventData) return;
    setForm({
      id: eventData.id,
      title: eventData.title,
      date: eventData.date.slice(0, 16),
      venue: eventData.venue,
      categoryId: eventData.categoryId,
      description: eventData.description || '',
      capacity: String(eventData.capacity),
    });
  }, [eventData]);

  const saveMutation = useMutation({
    mutationFn: updated => updateEvent(id, updated),
    onSuccess: () => {
      setSnackbar({ open: true, message: 'Event updated successfully', severity: 'success' });
      setTimeout(() => navigate('/admin/events', { replace: true }), 1500);
    },
    onError: err => setSnackbar({ open: true, message: err.message || 'Failed to save event', severity: 'error' }),
  });

  const handleChange = e => {
    const { name, value } = e.target;
    setForm(f => ({ ...f, [name]: value }));
  };

  const handleSubmit = e => {
    e.preventDefault();
    saveMutation.mutate({ ...form, capacity: parseInt(form.capacity, 10) });
  };

  const imagesData = !loadingImages && imagesList.length
    ? imagesList.map(img => ({
        id: img.id ?? img.Id,
        url: img.url ?? img.Url,
      }))
    : [{ id: 'placeholder', url: PLACEHOLDER_IMAGE }];

  const handleDeleteImage = async () => {
    const image = imagesData[currentSlide];
    if (image.id === 'placeholder') return;
    try {
      await deleteEventImage(id, image.id);
      setSnackbar({ open: true, message: 'Image deleted', severity: 'success' });
      queryClient.invalidateQueries(['eventImages', id]);
      setCurrentSlide(0);
    } catch (err) {
      setSnackbar({ open: true, message: err.message || 'Failed to delete image', severity: 'error' });
    }
  };

  if (loadingEvent || loadingCats || loadingImages) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
  if (isEventError) return <Container maxWidth="sm" sx={{ mt: 4 }}><Alert severity="error">Error loading event: {eventError.message}</Alert></Container>;
  if (isCatsError) return <Container maxWidth="sm" sx={{ mt: 4 }}><Alert severity="error">Error loading categories: {catsError.message}</Alert></Container>;
  if (isImagesError) console.warn('Error loading images:', imagesError);

  const sliderSettings = { dots: true, infinite: true, speed: 500, slidesToShow: 1, slidesToScroll: 1, beforeChange: (_, next) => setCurrentSlide(next) };

  return (
    <Container maxWidth="sm" sx={{ mt: 4, mb: 4 }}>
      <Typography variant="h5" gutterBottom>Edit Event</Typography>

      <Box mb={2}>
        {imagesData.length > 1 ? (
          <Slider {...sliderSettings}>
            {imagesData.map(img => (
              <CardMedia key={img.id} component="img" src={img.url} alt={form.title} height="200" sx={{ borderRadius: 1, mb: 2 }} />
            ))}
          </Slider>
        ) : (
          <CardMedia component="img" src={imagesData[0].url} alt={form.title} height="200" sx={{ borderRadius: 1, mb: 2 }} />
        )}
        <Button variant="outlined" color="error" onClick={handleDeleteImage} disabled={imagesData[currentSlide].id === 'placeholder'} sx={{ mt: 1 }}>
          Delete Photo
        </Button>
      </Box>

      <Box component="form" onSubmit={handleSubmit} noValidate sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
        <TextField label="Title" name="title" value={form.title} onChange={handleChange} required fullWidth />
        <TextField label="Date & Time" name="date" type="datetime-local" value={form.date} onChange={handleChange} required fullWidth InputLabelProps={{ shrink: true }} />
        <TextField label="Venue" name="venue" value={form.venue} onChange={handleChange} required fullWidth />
        <FormControl fullWidth>
          <InputLabel>Category</InputLabel>
          <Select label="Category" name="categoryId" value={form.categoryId} onChange={handleChange} required>
            <MenuItem value=""><em>Select a category</em></MenuItem>
            {categories.map(cat => <MenuItem key={cat.id} value={cat.id}>{cat.name}</MenuItem>)}
          </Select>
        </FormControl>
        <TextField label="Description" name="description" value={form.description} onChange={handleChange} multiline rows={4} fullWidth />
        <TextField label="Capacity" name="capacity" type="number" inputProps={{ min: 0 }} value={form.capacity} onChange={handleChange} required fullWidth />

        <Box sx={{ display: 'flex', justifyContent: 'space-between', mt: 2 }}>
          <Button type="submit" variant="contained" disabled={saveMutation.isLoading} startIcon={saveMutation.isLoading && <CircularProgress size={20} />}>
            {saveMutation.isLoading ? 'Saving…' : 'Save'}
          </Button>
          <Button variant="outlined" onClick={() => navigate('/admin/events', { replace: true })}>Cancel</Button>
        </Box>
      </Box>

      <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(s => ({ ...s, open: false }))} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        <Alert onClose={() => setSnackbar(s => ({ ...s, open: false }))} severity={snackbar.severity} sx={{ width: '100%' }}>
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Container>
  );
}
