// src/components/AdminEventsGrid.jsx
import React from 'react';
import { Grid, Button } from '@mui/material';
import EventCard from './EventCard';
import { Link } from 'react-router-dom';

export default function AdminEventsGrid({
  events,
  imageMap,
  onDelete,
  deleteLoadingId,
  onUpload,
  uploadLoadingId
}) {
  return (
    <Grid container spacing={3} wrap="wrap">
      {events.map(evt => {
        const isDeleting = deleteLoadingId === evt.id;
        const isUploading = uploadLoadingId === evt.id;

        return (
        <Grid
          item
          key={evt.id}
          sx={{
            // вычитаем по 24px (2 * 12px) из каждого 25%
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
            <EventCard evt={evt} imageUrl={imageMap[evt.id]} clickable={false}>
              <Button size="small" component={Link} to={`/admin/events/edit/${evt.id}`}>
                Edit
              </Button>
              <Button
                size="small"
                color="error"
                disabled={isDeleting}
                onClick={() => onDelete(evt.id)}
              >
                {isDeleting ? 'Deleting…' : 'Delete'}
              </Button>
              <Button
                size="small"
                component="label"
                disabled={isUploading}
              >
                {isUploading ? 'Uploading…' : 'Upload Image'}
                <input
                  type="file"
                  hidden
                  accept="image/*"
                  onChange={e => onUpload(evt.id, e.target.files[0])}
                />
              </Button>
            </EventCard>
          </Grid>
        );
      })}
    </Grid>
  );
}
