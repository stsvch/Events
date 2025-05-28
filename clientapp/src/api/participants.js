import api from './axiosInstance';

export function getParticipants(eventId) {
  const params = eventId ? { eventId } : {};
  return api.get('/participants', { params });
}

export function registerParticipant({ eventId }) {
  return api.post('/participants/register', { eventId });
}

export function unregisterParticipant(participantId, eventId) {
  return api.delete(`/participants/${participantId}`, {
    params: { eventId },
  });
}

export function isRegistered(eventId) {
  return api.get('/participants/is-registered', {
    params: { eventId },
  });
}