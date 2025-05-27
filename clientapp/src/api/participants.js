import api from './axiosInstance';

export function getParticipants(eventId) {
  const params = eventId ? { eventId } : {};
  return api.get('/participants', { params });
}

export function registerParticipant({ eventId }) {
  // server will pick up userId from the JWT, no other fields needed
  return api.post('/participants/register', { eventId });
}

export function unregisterParticipant(participantId, eventId) {
  return api.delete(`/participants/${participantId}`, {
    params: { eventId },
  });
}
