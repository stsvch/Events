import api from './axiosInstance';

export function getParticipants(eventId) {
  const params = eventId ? { eventId } : {};
  return api.get('/participants', { params });
}

export function registerParticipant({ eventId }) {
  return api.post('/participants/register', { eventId });
}

export function unregisterParticipant(eventId) {
  return api.post('/participants/unregister', { eventId });
}


export function isRegistered(eventId) {
  return api.get('/participants/is-registered', {
    params: { eventId },
  });
}

export function getParticipantById(id) {
  return api.get(`/participants/${id}`);
}

export function getCurrentParticipant() {
  return api.get('/participants/me');
}