import { useNavigate } from 'react-router-dom'
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
}
