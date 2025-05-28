// src/components/EventCard.jsx
import React from 'react';
import {
  Card,
  CardMedia,
  CardContent,
  CardActions,
  CardActionArea,
  Typography,
  Box,
  IconButton,
  Button
} from '@mui/material';
import { FavoriteBorder, Share, Schedule, LocationOn } from '@mui/icons-material';
import { Link } from 'react-router-dom';

const PLACEHOLDER = '/images/placeholder.png';

export default function EventCard({
  evt,
  imageUrl,
  children,    
  clickable = true, 
}) {
  const CardWrapper = clickable ? CardActionArea : React.Fragment;
  const wrapperProps = clickable ? { component: Link, to: `/events/${evt.id}` } : {};

  return (
    <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <CardWrapper {...wrapperProps} sx={{ flexGrow: 1 }}>
        <Box position="relative">
          <CardMedia
            component="img"
            image={imageUrl || PLACEHOLDER}
            alt={evt.title}
            sx={{
              width: '100%',
              aspectRatio: '16/9',
              objectFit: 'cover'
            }}
          />
          <Box
            position="absolute" top={8} left={8}
            bgcolor="rgba(0,0,0,0.6)" color="#fff"
            px={1} borderRadius={1} fontSize="0.75rem" fontWeight="bold"
          >
            {evt.availability}
          </Box>
          <Box position="absolute" top={4} right={4} display="flex">
            <IconButton size="small" sx={{ color: '#fff' }}><Share fontSize="small" /></IconButton>
            <IconButton size="small" sx={{ color: '#fff' }}><FavoriteBorder fontSize="small" /></IconButton>
          </Box>
        </Box>

        <CardContent>
          <Typography variant="subtitle2" color="textSecondary" gutterBottom>
            {new Date(evt.date).toLocaleDateString('en-US', { weekday: 'short', day: 'numeric', month: 'short' })}
          </Typography>
          <Typography variant="h6" gutterBottom noWrap>{evt.title}</Typography>
          <Box display="flex" alignItems="center" mb={0.5}>
            <Schedule fontSize="small" sx={{ mr: 0.5 }} />
            <Typography variant="body2">
              {new Date(evt.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
            </Typography>
          </Box>
          <Box display="flex" alignItems="center">
            <LocationOn fontSize="small" sx={{ mr: 0.5 }} />
            <Typography variant="body2" noWrap>{evt.venue}</Typography>
          </Box>
        </CardContent>
      </CardWrapper>

      {children && (
        <CardActions sx={{ mt: 'auto' }}>
          {children}
        </CardActions>
      )}
    </Card>
  );
}
