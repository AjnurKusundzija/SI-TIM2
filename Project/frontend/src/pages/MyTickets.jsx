import { useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { getMyTickets } from '../services/ticketService'
import { Search, Plus, Ticket } from 'lucide-react'
import EmptyState from '../components/common/EmptyState'

const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }
const STATUS_LABELS = { OPEN: 'Otvoren', CLOSED: 'Zatvoren', PENDING_CLOSE: 'Čeka zatvaranje' }
const TYPE_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobilna mreža',
  BILLING: 'Računi/Naplata',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
}

const STATUS_CLASSES = {
  OPEN: 'bg-emerald-100 text-emerald-800',
  CLOSED: 'bg-gray-100 text-gray-800',
  PENDING_CLOSE: 'bg-amber-100 text-amber-800',
}

const PRIORITY_CLASSES = {
  HIGH: 'bg-red-100 text-red-800',
  MEDIUM: 'bg-yellow-100 text-yellow-800',
  LOW: 'bg-blue-100 text-blue-800',
}

const EMPTY_FILTERS = { priority: '', status: '', type: '', dateFrom: '' }

export default function MyTickets() {
  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [filters, setFilters] = useState(EMPTY_FILTERS)
  const [search, setSearch] = useState('')

  useEffect(() => {
    getMyTickets()
      .then(setTickets)
      .catch((err) => { console.error(err); setError('Failed to load tickets.') })
      .finally(() => setLoading(false))
  }, [])

  const filteredTickets = useMemo(() => {
    return tickets.filter((t) => {
      if (filters.priority && t.priority !== filters.priority) return false
      if (filters.status && t.status !== filters.status) return false
      if (filters.type && t.problemCategory !== filters.type) return false
      if (filters.dateFrom && new Date(t.createdDate) < new Date(filters.dateFrom)) return false
      if (search) {
        const s = search.toLowerCase()
        if (!t.title?.toLowerCase().includes(s) && !t.subject?.toLowerCase().includes(s)) return false
      }
      return true
    })
  }, [tickets, filters, search])

  const activeFilters = Object.entries(filters).filter(([, v]) => v !== '')
  const removeFilter = (key) => setFilters((f) => ({ ...f, [key]: '' }))

  const filterLabel = (key, value) => {
    if (key === 'priority') return `Priority: ${PRIORITY_LABELS[value] ?? value}`
    if (key === 'status') return `Status: ${STATUS_LABELS[value] ?? value}`
    if (key === 'type') return `Type: ${TYPE_LABELS[value] ?? value}`
    if (key === 'dateFrom') return `From: ${new Date(value).toLocaleDateString()}`
    return value
  }

  return (
    <div className="space-y-4">
      {/* Toolbar */}
      <div className="flex flex-col sm:flex-row gap-3 items-start sm:items-center justify-between">
        <div className="relative flex-1 w-full sm:max-w-xs">
          <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            type="text"
            placeholder="Pretraži tikete..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-9 pr-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
          />
        </div>
        <div className="flex flex-wrap gap-2 items-center">
          <select
            value={filters.priority}
            onChange={(e) => setFilters((f) => ({ ...f, priority: e.target.value }))}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="">Svi prioriteti</option>
            {Object.entries(PRIORITY_LABELS).map(([val, label]) => (
              <option key={val} value={val}>{label}</option>
            ))}
          </select>

          <select
            value={filters.status}
            onChange={(e) => setFilters((f) => ({ ...f, status: e.target.value }))}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="">Svi statusi</option>
            {Object.entries(STATUS_LABELS).map(([val, label]) => (
              <option key={val} value={val}>{label}</option>
            ))}
          </select>

          <select
            value={filters.type}
            onChange={(e) => setFilters((f) => ({ ...f, type: e.target.value }))}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="">Svi tipovi</option>
            {Object.entries(TYPE_LABELS).map(([val, label]) => (
              <option key={val} value={val}>{label}</option>
            ))}
          </select>

          <input
            type="date"
            value={filters.dateFrom}
            onChange={(e) => setFilters((f) => ({ ...f, dateFrom: e.target.value }))}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          />

          <Link to="/create-ticket">
            <button className="flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-800 text-white font-medium rounded-lg text-sm transition-colors whitespace-nowrap">
              <Plus size={16} />
              Novi tiket
            </button>
          </Link>
        </div>
      </div>

      {/* Active filter chips */}
      {activeFilters.length > 0 && (
        <div className="flex flex-wrap items-center gap-2">
          {activeFilters.map(([key, value]) => (
            <span
              key={key}
              className="inline-flex items-center gap-1.5 px-3 py-1 bg-navy-50 border border-navy-200 rounded-full text-xs font-medium text-navy-700"
            >
              {filterLabel(key, value)}
              <button
                onClick={() => removeFilter(key)}
                className="text-navy-400 hover:text-navy-700 leading-none"
              >
                ×
              </button>
            </span>
          ))}
          <button
            onClick={() => setFilters(EMPTY_FILTERS)}
            className="text-xs text-red-600 hover:text-red-800 font-medium"
          >
            Očisti sve
          </button>
        </div>
      )}

      {/* Content */}
      {loading ? (
        <div className="flex justify-center py-16">
          <div className="w-8 h-8 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
        </div>
      ) : error ? (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">{error}</div>
      ) : filteredTickets.length === 0 ? (
        <EmptyState
          icon={Ticket}
          title={tickets.length === 0 ? 'Još nema tiketa' : 'Nijedan tiket ne odgovara filterima'}
          description={tickets.length === 0 ? 'Kreirajte vaš prvi tiket za podršku da biste započeli.' : 'Pokušajte prilagoditi ili očistiti filtere.'}
          action={tickets.length === 0 ? () => window.location.href = '/create-ticket' : undefined}
          actionLabel={tickets.length === 0 ? 'Kreiraj tiket' : undefined}
        />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          {/* Desktop table */}
          <div className="hidden md:block overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-100 bg-gray-50">
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Tiket</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Status</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Prioritet</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Tip</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Kreirano</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {filteredTickets.map((ticket) => (
                  <tr key={ticket.ticketId} className="hover:bg-gray-50 transition-colors">
                    <td className="px-5 py-3">
                      <p className="text-sm font-medium text-gray-900 truncate max-w-xs">
                        {ticket.title || ticket.subject}
                      </p>
                      <p className="text-xs text-gray-400">{ticket.ticketId}</p>
                    </td>
                    <td className="px-5 py-3">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${STATUS_CLASSES[ticket.status] || 'bg-gray-100 text-gray-800'}`}>
                        {STATUS_LABELS[ticket.status] ?? ticket.status}
                      </span>
                    </td>
                    <td className="px-5 py-3">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${PRIORITY_CLASSES[ticket.priority] || 'bg-gray-100 text-gray-800'}`}>
                        {PRIORITY_LABELS[ticket.priority] ?? ticket.priority}
                      </span>
                    </td>
                    <td className="px-5 py-3 text-sm text-gray-600">
                      {TYPE_LABELS[ticket.problemCategory] ?? ticket.problemCategory}
                    </td>
                    <td className="px-5 py-3 text-sm text-gray-500 whitespace-nowrap">
                      {ticket.createdDate
                        ? new Date(ticket.createdDate).toLocaleDateString()
                        : '—'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Mobile cards */}
          <div className="md:hidden divide-y divide-gray-50">
            {filteredTickets.map((ticket) => (
              <div key={ticket.ticketId} className="px-4 py-3">
                <p className="text-sm font-medium text-gray-900 truncate">
                  {ticket.title || ticket.subject}
                </p>
                <div className="flex flex-wrap items-center gap-2 mt-2">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${STATUS_CLASSES[ticket.status] || 'bg-gray-100 text-gray-800'}`}>
                    {STATUS_LABELS[ticket.status] ?? ticket.status}
                  </span>
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${PRIORITY_CLASSES[ticket.priority] || 'bg-gray-100 text-gray-800'}`}>
                    {PRIORITY_LABELS[ticket.priority] ?? ticket.priority}
                  </span>
                </div>
                <p className="text-xs text-gray-400 mt-1">
                  {ticket.createdDate
                    ? new Date(ticket.createdDate).toLocaleDateString()
                    : '—'}
                </p>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}
