// src/api/events.js
import api from './axiosInstance';

export function getEvents({
  title,
  pageNumber = 1,
  pageSize = 4,
  startDate,
  endDate,
  venue,
  categoryId,
  combineMode = 'And' 
}) {
  const params = { pageNumber, pageSize, combineMode };

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
  return api.put(`/events/${eventId}`, { id: eventId, ...eventData });
}

export function deleteEvent(eventId) {
  return api.delete(`/events/${eventId}`);
}

export function getMyEvents() {
  return api.get('/events/me');
}
