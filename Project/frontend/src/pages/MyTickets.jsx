import { useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { getMyTickets } from '../services/ticketService'

const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }
const STATUS_LABELS = { OPEN: 'Otvoreno', CLOSED: 'Zatvoreno' }
const TYPE_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobilna mreža',
  BILLING: 'Naplata',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
}

const EMPTY_FILTERS = { priority: '', status: '', type: '', dateFrom: '' }

export default function MyTickets() {
  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [filters, setFilters] = useState(EMPTY_FILTERS)

  useEffect(() => {
    getMyTickets()
      .then(setTickets)
      .catch((err) => { console.error(err); setError('Greška pri dohvaćanju tiketa.') })
      .finally(() => setLoading(false))
  }, [])

  const filteredTickets = useMemo(() => {
    return tickets.filter((t) => {
      if (filters.priority && t.priority !== filters.priority) return false
      if (filters.status && t.status !== filters.status) return false
      if (filters.type && t.problemCategory !== filters.type) return false
      if (filters.dateFrom && new Date(t.createdDate) < new Date(filters.dateFrom)) return false
      return true
    })
  }, [tickets, filters])

  const activeFilters = Object.entries(filters).filter(([, v]) => v !== '')

  const removeFilter = (key) => setFilters((f) => ({ ...f, [key]: '' }))

  const filterLabel = (key, value) => {
    if (key === 'priority') return `Prioritet: ${PRIORITY_LABELS[value] ?? value}`
    if (key === 'status') return `Status: ${STATUS_LABELS[value] ?? value}`
    if (key === 'type') return `Vrsta: ${TYPE_LABELS[value] ?? value}`
    if (key === 'dateFrom') return `Od: ${new Date(value).toLocaleDateString('bs-BA')}`
    return value
  }

  const getStatusStyle = (status) => {
    switch (status) {
      case 'OPEN':   return { color: '#10b981', background: '#ecfdf5' }
      case 'CLOSED': return { color: '#6b7280', background: '#f3f4f6' }
      default:       return { color: '#6b7280', background: '#f3f4f6' }
    }
  }

  const getPriorityStyle = (priority) => {
    switch (priority) {
      case 'HIGH':   return { color: '#ef4444', background: '#fef2f2' }
      case 'MEDIUM': return { color: '#f59e0b', background: '#fffbeb' }
      case 'LOW':    return { color: '#3b82f6', background: '#eff6ff' }
      default:       return { color: '#6b7280', background: '#f3f4f6' }
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        <div style={styles.header}>
          <div>
            <h1 style={styles.title}>Moji tiketi</h1>
            <p style={styles.subtitle}>Pregled svih vaših prijavljenih zahtjeva</p>
          </div>
          <Link to="/create-ticket">
            <button style={styles.createButton}>+ Kreiraj novi tiket</button>
          </Link>
        </div>

        {/* Filter panel */}
        <div style={styles.filterPanel}>
          <div style={styles.filterRow}>
            <select
              style={styles.select}
              value={filters.priority}
              onChange={(e) => setFilters((f) => ({ ...f, priority: e.target.value }))}
            >
              <option value="">Svi prioriteti</option>
              {Object.entries(PRIORITY_LABELS).map(([val, label]) => (
                <option key={val} value={val}>{label}</option>
              ))}
            </select>

            <select
              style={styles.select}
              value={filters.status}
              onChange={(e) => setFilters((f) => ({ ...f, status: e.target.value }))}
            >
              <option value="">Svi statusi</option>
              {Object.entries(STATUS_LABELS).map(([val, label]) => (
                <option key={val} value={val}>{label}</option>
              ))}
            </select>

            <select
              style={styles.select}
              value={filters.type}
              onChange={(e) => setFilters((f) => ({ ...f, type: e.target.value }))}
            >
              <option value="">Sve vrste</option>
              {Object.entries(TYPE_LABELS).map(([val, label]) => (
                <option key={val} value={val}>{label}</option>
              ))}
            </select>

            <input
              type="date"
              style={styles.dateInput}
              value={filters.dateFrom}
              onChange={(e) => setFilters((f) => ({ ...f, dateFrom: e.target.value }))}
            />

            {activeFilters.length > 0 && (
              <button style={styles.clearBtn} onClick={() => setFilters(EMPTY_FILTERS)}>
                Očisti filtere
              </button>
            )}
          </div>

          {activeFilters.length > 0 && (
            <div style={styles.chipRow}>
              {activeFilters.map(([key, value]) => (
                <span key={key} style={styles.chip}>
                  {filterLabel(key, value)}
                  <button style={styles.chipClose} onClick={() => removeFilter(key)}>×</button>
                </span>
              ))}
            </div>
          )}
        </div>

        {loading ? (
          <p style={styles.info}>Učitavanje tiketa...</p>
        ) : error ? (
          <p style={{ ...styles.info, color: '#ef4444' }}>{error}</p>
        ) : filteredTickets.length > 0 ? (
          <div style={styles.list}>
            {filteredTickets.map((ticket) => (
              <div key={ticket.ticketId} style={styles.ticketCard}>
                <div style={styles.ticketInfo}>
                  <h3 style={styles.ticketTitle}>{ticket.title}</h3>
                  <div style={styles.ticketMeta}>
                    <span style={styles.date}>
                      {new Date(ticket.createdDate).toLocaleDateString('bs-BA')}
                    </span>
                    <span style={{ ...styles.badge, ...getPriorityStyle(ticket.priority) }}>
                      {PRIORITY_LABELS[ticket.priority] ?? ticket.priority}
                    </span>
                    <span style={styles.category}>
                      {TYPE_LABELS[ticket.problemCategory] ?? ticket.problemCategory}
                    </span>
                  </div>
                </div>
                <div style={{ ...styles.statusBadge, ...getStatusStyle(ticket.status) }}>
                  {STATUS_LABELS[ticket.status] ?? ticket.status}
                </div>
              </div>
            ))}
          </div>
        ) : tickets.length === 0 ? (
          <div style={styles.emptyState}>
            <p>Nemate otvorenih tiketa.</p>
          </div>
        ) : (
          <div style={styles.emptyState}>
            <p style={{ fontWeight: 600, color: '#374151', marginBottom: 6 }}>
              Nema tiketa koji odgovaraju odabranim filterima.
            </p>
            <p style={{ fontSize: 13, margin: 0 }}>Pokušajte promijeniti ili ukloniti filtere.</p>
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
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: '24px',
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
  createButton: {
    padding: '10px 20px',
    background: '#3b82f6',
    color: '#ffffff',
    border: 'none',
    borderRadius: '8px',
    fontSize: '14px',
    fontWeight: 500,
    cursor: 'pointer',
  },
  filterPanel: {
    background: '#ffffff',
    border: '1px solid #e5e7eb',
    borderRadius: '12px',
    padding: '16px 20px',
    marginBottom: '24px',
    display: 'flex',
    flexDirection: 'column',
    gap: '12px',
  },
  filterRow: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '10px',
    alignItems: 'center',
  },
  select: {
    padding: '8px 12px',
    border: '1px solid #d1d5db',
    borderRadius: '8px',
    fontSize: '14px',
    background: '#f9fafb',
    color: '#374151',
    cursor: 'pointer',
  },
  dateInput: {
    padding: '8px 12px',
    border: '1px solid #d1d5db',
    borderRadius: '8px',
    fontSize: '14px',
    background: '#f9fafb',
    color: '#374151',
  },
  clearBtn: {
    padding: '8px 14px',
    border: '1px solid #fca5a5',
    borderRadius: '8px',
    fontSize: '13px',
    background: '#fef2f2',
    color: '#ef4444',
    cursor: 'pointer',
    fontWeight: 500,
  },
  chipRow: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '8px',
  },
  chip: {
    display: 'inline-flex',
    alignItems: 'center',
    gap: '6px',
    padding: '4px 10px',
    background: '#eff6ff',
    border: '1px solid #bfdbfe',
    borderRadius: '999px',
    fontSize: '12px',
    color: '#1d4ed8',
    fontWeight: 500,
  },
  chipClose: {
    background: 'none',
    border: 'none',
    cursor: 'pointer',
    color: '#1d4ed8',
    fontSize: '16px',
    lineHeight: 1,
    padding: 0,
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
  },
  ticketInfo: {
    display: 'flex',
    flexDirection: 'column',
    gap: '6px',
  },
  ticketTitle: {
    margin: 0,
    fontSize: '16px',
    fontWeight: 600,
    color: '#111827',
  },
  ticketMeta: {
    display: 'flex',
    alignItems: 'center',
    gap: '10px',
    flexWrap: 'wrap',
  },
  date: {
    fontSize: '13px',
    color: '#9ca3af',
  },
  badge: {
    padding: '2px 10px',
    borderRadius: '999px',
    fontSize: '12px',
    fontWeight: 600,
  },
  category: {
    fontSize: '12px',
    color: '#6b7280',
  },
  statusBadge: {
    padding: '6px 14px',
    borderRadius: '999px',
    fontSize: '12px',
    fontWeight: 700,
    letterSpacing: '0.4px',
    whiteSpace: 'nowrap',
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
