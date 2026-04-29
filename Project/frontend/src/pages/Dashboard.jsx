/*import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Dashboard() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login')
  }

  return (
    <div style={{ padding: '40px' }}>
      <h1>Dashboard</h1>
      <p>Ulogovan kao: <strong>{user?.email}</strong> ({user?.role})</p>
      <button onClick={handleLogout} style={{ marginTop: '16px', padding: '8px 16px', cursor: 'pointer' }}>
        Logout
      </button>
    </div>
  )
} */

import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export default function Dashboard() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login')
  }

  return (
    <div style={{ padding: '40px' }}>
      <h1>Dashboard</h1>
      <p>Ulogovan kao: <strong>{user?.email}</strong> ({user?.role})</p>
      
      <div style={{ display: 'flex', gap: '10px', marginTop: '16px' }}>
        {/* DODAN LINK DO TIKETA */}
        <Link to="/mytickets">
          <button style={{ padding: '8px 16px', cursor: 'pointer' }}>
            Moji tiketi
          </button>
        </Link>

        <Link to="/create-ticket">
          <button style={{ padding: '8px 16px', cursor: 'pointer', backgroundColor: '#10b981', color: '#fff', border: 'none' }}>
            Kreiraj tiket
          </button>
        </Link>

        <button 
          onClick={handleLogout} 
          style={{ padding: '8px 16px', cursor: 'pointer', backgroundColor: '#fff', border: '1px solid #ccc', color: '#111827' }}
        >
          Logout
        </button>
      </div>
    </div>
  )
}