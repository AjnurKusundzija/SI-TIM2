// Lokacija: src/pages/MojiTiketi.jsx
// BUGFIX 1: ticket.id → ticket.ticketId  (usklađeno sa MyTicketDto)
// BUGFIX 2: Ruta u App.jsx mora biti unutar <ProtectedRoute>

import { useState, useEffect } from 'react'
import { getMyTickets } from '../services/ticketService'

export default function MyTickets() {
  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    const fetchTickets = async () => {
      try {
        const data = await getMyTickets()  // koristi ticketService.js koji ima api instancu s interceptorima
        setTickets(data)
      } catch (err) {
        console.error(err)
        setError('Greška pri dohvaćanju tiketa.')
      } finally {
        setLoading(false)
      }
    }

    fetchTickets()
  }, [])

  const getStatusStyle = (status) => {
    switch (status) {
      case 'OPEN':       return { color: '#10b981', background: '#ecfdf5' }
      case 'IN_PROGRESS': return { color: '#f59e0b', background: '#fffbeb' }
      case 'CLOSED':     return { color: '#6b7280', background: '#f3f4f6' }
      default:           return { color: '#6b7280', background: '#f3f4f6' }
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        <div style={styles.header}>
          <h1 style={styles.title}>Moji tiketi</h1>
          <p style={styles.subtitle}>Pregled svih vaših prijavljenih zahtjeva</p>
        </div>

        {loading ? (
          <p style={styles.info}>Učitavanje tiketa...</p>
        ) : error ? (
          <p style={{ ...styles.info, color: '#ef4444' }}>{error}</p>
        ) : tickets.length > 0 ? (
          <div style={styles.list}>
            {tickets.map((ticket) => (
              // BUGFIX: ticket.ticketId umjesto ticket.id
              <div key={ticket.ticketId} style={styles.ticketCard}>
                <div style={styles.ticketInfo}>
                  <h3 style={styles.ticketTitle}>{ticket.title}</h3>
                  <span style={styles.date}>
                    {new Date(ticket.createdDate).toLocaleDateString('bs-BA')}
                  </span>
                </div>
                <div style={{ ...styles.statusBadge, ...getStatusStyle(ticket.status) }}>
                  {ticket.status}
                </div>
              </div>
            ))}
          </div>
        ) : (
          <div style={styles.emptyState}>
            <p>Nemate otvorenih tiketa.</p>
          </div>
        )}
      </div>
    </div>
  )
}

const styles = {
  page: {
    minHeight: '100vh',
    padding: '40px 20px',
    background: '#f9fafb',
    display: 'flex',
    justifyContent: 'center',
  },
  container: {
    width: '100%',
    maxWidth: '900px',
  },
  header: {
    marginBottom: '32px',
  },
  title: {
    fontSize: '30px',
    margin: '0 0 6px',
    fontWeight: 700,
    color: '#111827',
  },
  subtitle: {
    fontSize: '14px',
    color: '#6b7280',
    margin: 0,
  },
  list: {
    display: 'flex',
    flexDirection: 'column',
    gap: '14px',
  },
  ticketCard: {
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: '18px 20px',
    background: '#ffffff',
    border: '1px solid #e5e7eb',
    borderRadius: '14px',
    boxShadow: '0 4px 10px rgba(0,0,0,0.04)',
    cursor: 'pointer',
    transition: 'all 0.2s ease',
  },
  ticketInfo: {
    display: 'flex',
    flexDirection: 'column',
    gap: '4px',
  },
  ticketTitle: {
    margin: 0,
    fontSize: '16px',
    fontWeight: 600,
    color: '#111827',
  },
  date: {
    fontSize: '13px',
    color: '#9ca3af',
  },
  statusBadge: {
    padding: '6px 14px',
    borderRadius: '999px',
    fontSize: '12px',
    fontWeight: 700,
    letterSpacing: '0.4px',
  },
  info: {
    color: '#6b7280',
    textAlign: 'center',
  },
  emptyState: {
    padding: '50px',
    textAlign: 'center',
    border: '2px dashed #e5e7eb',
    borderRadius: '12px',
    color: '#6b7280',
    background: '#ffffff',
  },
}
