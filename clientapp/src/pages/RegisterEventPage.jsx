import React, { useState, useEffect } from 'react';
import { useParams, useHistory, Link } from 'react-router-dom';
import { getEventById } from '../api/events';
import { registerParticipant } from '../api/participants';

export default function RegisterEventPage() {
  const { id: eventId } = useParams();
  const history = useHistory();

  const [event, setEvent] = useState(null);
  const [loadingEvent, setLoadingEvent] = useState(true);
  const [loadingSubmit, setLoadingSubmit] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEvent = async () => {
      setLoadingEvent(true);
      setError(null);
      try {
        const { data } = await getEventById(eventId);
        setEvent(data);
      } catch (err) {
        setError(err.message || 'Failed to load event');
      } finally {
        setLoadingEvent(false);
      }
    };
    fetchEvent();
  }, [eventId]);

  const handleSubmit = async e => {
    e.preventDefault();
    setLoadingSubmit(true);
    setError(null);
    try {
      await registerParticipant({ eventId });
      history.push('/my-events');
    } catch (err) {
      setError(err.message || 'Registration failed');
    } finally {
      setLoadingSubmit(false);
    }
  };

  if (loadingEvent) return <p className="text-center mt-8">Loading event details…</p>;
  if (error && !event) {
    return (
      <div className="text-center mt-8 text-red-600">
        <p>{error}</p>
        <Link to="/events" className="mt-4 inline-block text-blue-600 hover:underline">
          Back to Events
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-lg mx-auto mt-8 p-6 bg-white rounded shadow">
      <h1 className="text-2xl font-bold mb-4">Register for "{event.title}"</h1>
      <p className="text-gray-600 mb-6">
        {new Date(event.date).toLocaleString()} — {event.venue}
      </p>

      {error && (
        <div className="mb-4 text-red-600 text-center">
          {error}
        </div>
      )}

      <form onSubmit={handleSubmit} className="space-y-4">
        <button
          type="submit"
          disabled={loadingSubmit}
          className="w-full px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 disabled:opacity-50"
        >
          {loadingSubmit ? 'Registering…' : 'Register'}
        </button>
      </form>

      <Link
        to={`/events/${eventId}`}
        className="mt-4 block text-center text-blue-600 hover:underline"
      >
        Cancel
      </Link>
    </div>
  );
}
