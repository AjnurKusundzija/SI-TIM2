import PropTypes from 'prop-types'
import { useState, useEffect, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { getAllTickets } from '../services/ticketService'
//import { useAuth } from '../context/AuthContext'
import { Search, Ticket } from 'lucide-react'
import EmptyState from '../components/common/EmptyState'

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

const STATUS_LABELS = { OPEN: 'Otvoren', CLOSED: 'Zatvoren', PENDING_CLOSE: 'Čeka zatvaranje' }
const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }
const TYPE_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobilna mreža',
  BILLING: 'Računi/Naplata',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
}

export default function Tickets({ assignedOnly = false }) {
  const navigate = useNavigate()
  //const { user } = useAuth()
  //const isAgent = user?.role === 'AGENT'

  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState(assignedOnly ? 'OPEN' : 'ALL')
  const [typeFilter, setTypeFilter] = useState('ALL')
  const [priorityFilter, setPriorityFilter] = useState('ALL')

  useEffect(() => {
    getAllTickets(assignedOnly)
      .then((data) => { setError(null); setTickets(data) })
      .catch((err) => { console.error(err); setError('Failed to load tickets.') })
      .finally(() => setLoading(false))
  }, [assignedOnly])

  const filtered = useMemo(() => {
    return tickets.filter((t) => {
      if (statusFilter !== 'ALL' && t.status !== statusFilter) return false
      if (typeFilter !== 'ALL' && t.problemCategory !== typeFilter) return false
      if (priorityFilter !== 'ALL' && t.priority !== priorityFilter) return false
      if (search) {
        const s = search.toLowerCase()
        if (!t.title?.toLowerCase().includes(s) && !t.subject?.toLowerCase().includes(s)) return false
      }
      return true
    })
  }, [tickets, search, statusFilter, typeFilter, priorityFilter])

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
        <div className="flex flex-wrap gap-2">
          <select
            value={statusFilter}
            onChange={(e) => setStatusFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="ALL">Svi statusi</option>
            {Object.entries(STATUS_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          <select
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="ALL">Svi tipovi</option>
            {Object.entries(TYPE_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          <select
            value={priorityFilter}
            onChange={(e) => setPriorityFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="ALL">Svi prioriteti</option>
            {Object.entries(PRIORITY_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
        </div>
      </div>

      {loading ? (
        <div className="flex justify-center py-16">
          <div className="w-8 h-8 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
        </div>
      ) : error ? (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">{error}</div>
      ) : filtered.length === 0 ? (
        <EmptyState icon={Ticket} title="Nema pronađenih tiketa" description="Nijedan tiket ne odgovara vašim filterima." />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
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
                {filtered.map((t) => (
                  <tr key={t.ticketId} onClick={() => navigate(`/tickets/${t.ticketId}`)} className="hover:bg-gray-50 transition-colors cursor-pointer">
                    <td className="px-5 py-3">
                      <p className="text-sm font-medium text-gray-900 truncate max-w-xs">{t.title || t.subject}</p>
                      <p className="text-xs text-gray-400">{t.ticketId}</p>
                    </td>
                    <td className="px-5 py-3">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${STATUS_CLASSES[t.status] || 'bg-gray-100 text-gray-800'}`}>
                        {STATUS_LABELS[t.status] ?? t.status}
                      </span>
                    </td>
                    <td className="px-5 py-3">
                      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${PRIORITY_CLASSES[t.priority] || 'bg-gray-100 text-gray-800'}`}>
                        {PRIORITY_LABELS[t.priority] ?? t.priority}
                      </span>
                    </td>
                    <td className="px-5 py-3 text-sm text-gray-600">
                      {TYPE_LABELS[t.problemCategory] ?? t.problemCategory}
                    </td>
                    <td className="px-5 py-3 text-sm text-gray-500 whitespace-nowrap">
                      {t.createdDate ? new Date(t.createdDate).toLocaleDateString() : '—'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="md:hidden divide-y divide-gray-50">
            {filtered.map((t) => (
              <div key={t.ticketId} onClick={() => navigate(`/tickets/${t.ticketId}`)} className="px-4 py-3 cursor-pointer hover:bg-gray-50">
                <p className="text-sm font-medium text-gray-900 truncate">{t.title || t.subject}</p>
                <div className="flex flex-wrap items-center gap-2 mt-2">
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${STATUS_CLASSES[t.status] || 'bg-gray-100 text-gray-800'}`}>
                    {STATUS_LABELS[t.status] ?? t.status}
                  </span>
                  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${PRIORITY_CLASSES[t.priority] || 'bg-gray-100 text-gray-800'}`}>
                    {PRIORITY_LABELS[t.priority] ?? t.priority}
                  </span>
                </div>
                <p className="text-xs text-gray-400 mt-1">
                  {t.createdDate ? new Date(t.createdDate).toLocaleDateString() : '—'}
                </p>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  )
}

Tickets.propTypes = {
  assignedOnly: PropTypes.bool,
}
