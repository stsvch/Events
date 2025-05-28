import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './context/AuthContext';
import Layout from './components/Layout';
import PrivateRoute from './utils/PrivateRoute';
import LoginPage from './pages/Auth/LoginPage';
import RegisterPage from './pages/Auth/RegisterPage';
import EventsListPage from './pages/Events/EventsListPage';
import EventDetailPage from './pages/Events/EventDetailPage';
import MyEventsPage from './pages/Events/MyEventsPage';
import AdminEventsPage from './pages/Events/AdminEventsPage';
import CreateEventPage from './pages/Events/CreateEventPage';
import EditEventPage from './pages/Events/EditEventPage';
import UnauthorizedPage from './pages/UnauthorizedPage';
import RegisterParticipantPage from './pages/Events/RegisterParticipantPage';

export default function Router() {
  const { user } = useAuth();

  return (
    <Layout>
      <Routes>
        {/* Public */}
        <Route path="/" element={<Navigate to="/events" replace />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/events" element={<EventsListPage />} />
        <Route path="/events/:id" element={<EventDetailPage />} />

        {/* Любой залогиненный */}
        <Route element={<PrivateRoute />}>
          <Route path="/my-events" element={<MyEventsPage />} />
          <Route
            path="/participants/register"
            element={<RegisterParticipantPage />}
          />
        </Route>

        {/* Только админ */}
        <Route element={<PrivateRoute roles={['Admin']} />}>
          <Route path="/admin/events" element={<AdminEventsPage />} />
          <Route path="/admin/events/create" element={<CreateEventPage />} />
          <Route path="/admin/events/edit/:id" element={<EditEventPage />} />
        </Route>

        {/* 403 */}
        <Route path="/unauthorized" element={<UnauthorizedPage />} />

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  );
}
