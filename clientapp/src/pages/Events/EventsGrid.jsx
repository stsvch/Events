import React from 'react';
import {
  Box,
  Typography,
  Grid,
  Card,
  CardMedia,
  CardContent,
  CardActions,
  CardActionArea,
  IconButton,
  Button
} from '@mui/material';
import { FavoriteBorder, Share, Schedule, LocationOn } from '@mui/icons-material';

import { Link } from 'react-router-dom';

const PLACEHOLDER = '/images/placeholder.png';

export default function EventsGrid({ events }) {
  return (
    <Grid container spacing={3}>
      {events.map(evt => (
        <Grid item key={evt.id} xs={12} sm={6} md={4} lg={3}>
          <Card>
            {/* Делаем весь верх карточки кликабельным */}
            <CardActionArea component={Link} to={`/events/${evt.id}`}>
              {/* Изображение + Availability + иконки */}
              <Box position="relative">
                <CardMedia
                  component="img"
                  height="140"
                  image={evt.firstImageUrl || PLACEHOLDER}
                  alt={evt.title}
                />
                <Box
                  position="absolute"
                  top={8}
                  left={8}
                  bgcolor="rgba(0,0,0,0.6)"
                  color="#fff"
                  px={1}
                  borderRadius={1}
                  fontSize="0.75rem"
                  fontWeight="bold"
                >
                  {evt.availability}
                </Box>
                <Box position="absolute" top={4} right={4} display="flex">
                  <IconButton size="small" sx={{ color: '#fff' }}>
                    <Share fontSize="small" />
                  </IconButton>
                  <IconButton size="small" sx={{ color: '#fff' }}>
                    <FavoriteBorder fontSize="small" />
                  </IconButton>
                </Box>
              </Box>

              {/* Контент карточки */}
              <CardContent>
                <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                  {new Date(evt.date).toLocaleDateString('en-US', {
                    weekday: 'short', day: 'numeric', month: 'short'
                  })}
                </Typography>
                <Typography variant="h6" component="div" gutterBottom noWrap>
                  {evt.title}
                </Typography>
                <Box display="flex" alignItems="center" mb={0.5}>
                  <Schedule fontSize="small" sx={{ mr: 0.5 }} />
                  <Typography variant="body2">
                    {new Date(evt.date).toLocaleTimeString([], {
                      hour: '2-digit', minute: '2-digit'
                    })}
                  </Typography>
                </Box>
                <Box display="flex" alignItems="center">
                  <LocationOn fontSize="small" sx={{ mr: 0.5 }} />
                  <Typography variant="body2" noWrap>
                    {evt.venue}
                  </Typography>
                </Box>
              </CardContent>
            </CardActionArea>

            {/* Кнопка Learn More (необязательно) */}
            <CardActions>
              <Button size="small" component={Link} to={`/events/${evt.id}`}>
                Learn More
              </Button>
            </CardActions>
          </Card>
        </Grid>
      ))}
    </Grid>
  );
}
