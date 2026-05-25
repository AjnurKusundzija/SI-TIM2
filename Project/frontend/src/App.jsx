import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom'
import { AuthProvider, useAuth } from './context/AuthContext'
import { NotificationProvider } from './context/NotificationContext'
import AppLayout from './components/layout/AppLayout'
import ProtectedRoute from './components/layout/ProtectedRoute'

import Home from './pages/Home'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import MyTickets from './pages/MyTickets'
import Tickets from './pages/Tickets'
import AssignedTickets from './pages/AssignedTickets'
import CreateTicket from './pages/CreateTicket'
import Faq from './pages/Faq'
import TicketDetail from './pages/TicketDetail'
import Statistics from './pages/Statistics'
import Notifications from './pages/Notifications'
import Packages from './pages/Packages'
import PackageDetail from './pages/PackageDetail'
import Profile from './pages/Profile'
import UserProfile from './pages/UserProfile'
import Reports from './pages/Reports'
import PackageManagement from './pages/PackageManagement'
import UsersList from './pages/UsersList'
import CreateUser from './pages/CreateUser'
import AuditLogPage from './pages/AuditLog/AuditLogPage'



function AppRoutes() {
  const { user } = useAuth()

  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route
        path="/login"
        element={user ? <Navigate to="/dashboard" replace /> : <Login />}
      />

      <Route
        element={
          <ProtectedRoute>
            <AppLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/mytickets" element={<MyTickets />} />
        <Route path="/mytickets/:id" element={<TicketDetail />} />
        <Route path="/tickets" element={<Tickets />} />
        <Route path="/tickets/:id" element={<TicketDetail />} />
        <Route path="/assigned" element={<AssignedTickets />} />
        <Route path="/create-ticket" element={<CreateTicket />} />
        <Route path="/faq" element={<Faq />} />
        <Route
          path="/reports"
          element={
            <ProtectedRoute allowedRoles={['ADMINISTRATOR']}>
              <Reports />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin/packages"
          element={
            <ProtectedRoute allowedRoles={['ADMINISTRATOR']}>
              <PackageManagement />
            </ProtectedRoute>
          }
        />
        <Route
          path="/audit-log"
          element={
            <ProtectedRoute allowedRoles={['ADMINISTRATOR']}>
              <AuditLogPage />
            </ProtectedRoute>
          }
        />
        <Route path="/statistics" element={<Statistics />} />
        <Route path="/notifications" element={<Notifications />} />

        <Route path="/packages" element={<Packages />} />
        <Route path="/packages/:id" element={<PackageDetail />} />
        <Route path="/users/:id" element={<UserProfile />} />
        <Route path="/profile" element={<Profile />} />
        
        {/* User Account Management Routes */}
        <Route path="/users/add" element={<ProtectedRoute allowedRoles={['ADMINISTRATOR']}><CreateUser /></ProtectedRoute>} />
        <Route path="/users/clients" element={<ProtectedRoute allowedRoles={['ADMINISTRATOR', 'AGENT']}><UsersList /></ProtectedRoute>} />
        <Route path="/users/agents" element={<ProtectedRoute allowedRoles={['ADMINISTRATOR']}><UsersList /></ProtectedRoute>} />
        <Route path="/users/technicians" element={<ProtectedRoute allowedRoles={['ADMINISTRATOR', 'AGENT']}><UsersList /></ProtectedRoute>} />
        <Route path="/users/deactivated" element={<ProtectedRoute allowedRoles={['ADMINISTRATOR']}><UsersList /></ProtectedRoute>} />

      </Route>

      <Route path="*" element={<Navigate to={user ? '/dashboard' : '/login'} replace />} />
    </Routes>
  )
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <NotificationProvider>
          <AppRoutes />
        </NotificationProvider>
      </AuthProvider>
    </BrowserRouter>
  )
}
