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