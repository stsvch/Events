// src/api/events.js
import api from './axiosInstance';

export function getEvents({
  title,
  pageNumber = 1,
  pageSize = 10,
  startDate,
  endDate,
  venue,
  categoryId
}) {
  const params = { pageNumber, pageSize };
  if (title)      params.title      = title;
  if (startDate)  params.startDate  = startDate;
  if (endDate)    params.endDate    = endDate;
  if (venue)      params.venue      = venue;
  if (categoryId) params.categoryId = categoryId;
  return api.get('/events', { params });
}

export function getEventById(eventId) {
  return api.get(`/events/${eventId}`);
}

export function createEvent(eventData, files = [], imageUrls = []) {
  const formData = new FormData();
  formData.append('title',       eventData.title);
  formData.append('date',        eventData.date);
  formData.append('venue',       eventData.venue);
  formData.append('categoryId',  eventData.categoryId);
  formData.append('description', eventData.description);
  formData.append('capacity',    eventData.capacity);
  files.forEach(file => formData.append('files', file));
  imageUrls.forEach(url => formData.append('imageUrls', url));

  return api.post('/events', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  });
}

export function updateEvent(eventId, eventData) {
  return api.put(`/events/${eventId}`, eventData);
}

export function deleteEvent(eventId) {
  return api.delete(`/events/${eventId}`);
}

export function uploadEventImage(eventId, { file, url }) {
  const formData = new FormData();
  if (file) {
    formData.append('file', file);
  } else if (url) {
    formData.append('url', url);
  }
  return api.post(
    `/events/${eventId}/images`,
    formData,
    { headers: { 'Content-Type': 'multipart/form-data' } }
  );
}

export function deleteEventImage(eventId, imageId) {
  return api.delete(`/events/${eventId}/images/${imageId}`);
}
