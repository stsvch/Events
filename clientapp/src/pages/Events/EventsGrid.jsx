// src/pages/Events/EventsGrid.jsx
import React from 'react';
import { Grid } from '@mui/material';
import EventCard from '../../components/EventCard';
import { Button } from '@mui/material';
import { Link } from 'react-router-dom';


export default function EventsGrid({ events, imageMap }) {
  return (
    <Grid container spacing={3} sx={{ justifyContent: 'flex-start' }}>
      {events.map(evt => (
        <Grid
          item
          key={evt.id}
          sx={{
            flex: '0 0 calc(25% - 24px)',
            maxWidth: 'calc(25% - 24px)',
            '@media (max-width:1200px)': {
              flex: '0 0 calc(33.33% - 24px)',
              maxWidth: 'calc(33.33% - 24px)',
            },
            '@media (max-width:900px)': {
              flex: '0 0 calc(50% - 24px)',
              maxWidth: 'calc(50% - 24px)',
            },
            '@media (max-width:600px)': {
              flex: '0 0 100%',
              maxWidth: '100%',
            },
          }}
        >
        </Grid>
      ))}
    </Grid>
  );
}
