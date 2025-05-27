import api from './axiosInstance';

export function getEvents({ pageNumber = 1, pageSize = 10, startDate, endDate, venue, categoryId }) {
  const params = { pageNumber, pageSize };
  if (startDate)  params.startDate = startDate;
  if (endDate)    params.endDate = endDate;
  if (venue)      params.venue = venue;
  if (categoryId) params.categoryId = categoryId;
  return api.get('/events', { params });
}

export function getEventById(eventId) {
  return api.get(`/events/${eventId}`);
}

export function createEvent(eventData) {
  return api.post('/events', eventData);
}

export function updateEvent(eventId, eventData) {
  return api.put(`/events/${eventId}`, eventData);
}

export function deleteEvent(eventId) {
  return api.delete(`/events/${eventId}`);
}


export function uploadEventImage(eventId, file) {
  const formData = new FormData();
  formData.append('file', file);

  return api.post(
    `/events/${eventId}/images`,
    formData,
    {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    }
  );
}

export function getEventSummary(eventId) {
  return api.get(`/events/${eventId}/summary`);
}
