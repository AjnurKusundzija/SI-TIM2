import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom"
import { AuthProvider } from "./context/AuthContext"
import ProtectedRoute from "./components/ProtectedRoute"
import Home from "./pages/Home"
import Login from "./pages/Login"
import Dashboard from "./pages/Dashboard"
import Tickets from "./pages/Tickets"
import MojiTiketi from "./pages/MojiTiketi"

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />

          {/* US-11: Zaštićena ruta — redirect na /login ako korisnik nije prijavljen */}
          <Route path="/mojitiketi" element={
            <ProtectedRoute><MojiTiketi /></ProtectedRoute>
          } />

          <Route path="/dashboard" element={
            <ProtectedRoute><Dashboard /></ProtectedRoute>
          } />
          <Route path="/Tickets" element={
            <ProtectedRoute><Tickets /></ProtectedRoute>
          } />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
