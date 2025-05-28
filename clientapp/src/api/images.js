// src/api/images.js
import api from './axiosInstance';

export function getEventImages(eventId) {
  return api.get(`/events/${eventId}/images`)
            .then(res => res.data);
}

export function getFirstEventImage(eventId) {
  return api.get(`/events/${eventId}/images/first`)
            .then(res => res.data.url);
}

export async function uploadEventImage(eventId, source) {
  const formData = new FormData();
  if (source instanceof File) {
    formData.append('file', source);
  } else {
    formData.append('url', source.trim());
  }
  const config = {
    headers: { 'Content-Type': 'multipart/form-data' },
  };
  const res = await api.post(`events/${eventId}/images`, formData, config);
  return res.data.url;
}

export async function deleteEventImage(eventId, imageId) {
  await api.delete(`/events/${eventId}/images/${imageId}`);
}
