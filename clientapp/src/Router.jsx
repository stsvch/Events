import React from 'react';
 import { BrowserRouter, Switch, Route, Redirect } from 'react-router-dom';
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
 import Layout from './components/Layout';

 export default function Router() {
   return (
     <BrowserRouter>
       <Layout>
         <Switch>
         {/* Public */}
         <Route path="/login" component={LoginPage} />
         <Route path="/register" component={RegisterPage} />
         <Route exact path="/" render={() => <Redirect to="/events" />} />
         <Route exact path="/events" component={EventsListPage} />
         <Route exact path="/events/:id" component={EventDetailPage} />

         {/* Protected user */}
         <PrivateRoute
           exact
           path="/my-events"
           component={MyEventsPage}
           roles={['RegisteredUser']}
         />

         {/* Admin */}
         <PrivateRoute
           exact
           path="/admin/events"
           component={AdminEventsPage}
           roles={['Admin']}
         />
         <PrivateRoute
           exact
           path="/admin/events/create"
           component={CreateEventPage}
           roles={['Admin']}
         />
         <PrivateRoute
           exact
           path="/admin/events/edit/:id"
           component={EditEventPage}
           roles={['Admin']}
         />

         {/* Unauthorized */}
         <Route path="/unauthorized" component={UnauthorizedPage} />

         {/* Fallback */}
         <Redirect to="/" />
         </Switch>
       </Layout>
     </BrowserRouter>
   );
 }
