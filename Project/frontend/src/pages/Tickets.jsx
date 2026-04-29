import { useState, useEffect, useMemo } from 'react'
import { getMyTickets } from '../services/ticketService'
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

const STATUS_LABELS = { OPEN: 'Open', CLOSED: 'Closed', PENDING_CLOSE: 'Pending Close' }
const PRIORITY_LABELS = { LOW: 'Low', MEDIUM: 'Medium', HIGH: 'High' }
const TYPE_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobile Network',
  BILLING: 'Billing',
  TECHNICAL_SUPPORT: 'Technical Support',
}

export default function Tickets() {
  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [search, setSearch] = useState('')
  const [statusFilter, setStatusFilter] = useState('ALL')
  const [typeFilter, setTypeFilter] = useState('ALL')
  const [priorityFilter, setPriorityFilter] = useState('ALL')

  useEffect(() => {
    getMyTickets()
      .then(setTickets)
      .catch((err) => { console.error(err); setError('Failed to load tickets.') })
      .finally(() => setLoading(false))
  }, [])

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
            placeholder="Search tickets..."
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
            <option value="ALL">All Status</option>
            {Object.entries(STATUS_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          <select
            value={typeFilter}
            onChange={(e) => setTypeFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="ALL">All Types</option>
            {Object.entries(TYPE_LABELS).map(([v, l]) => <option key={v} value={v}>{l}</option>)}
          </select>
          <select
            value={priorityFilter}
            onChange={(e) => setPriorityFilter(e.target.value)}
            className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none"
          >
            <option value="ALL">All Priority</option>
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
        <EmptyState icon={Ticket} title="No tickets found" description="No tickets match your current filters." />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="hidden md:block overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b border-gray-100 bg-gray-50">
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Ticket</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Status</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Priority</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Type</th>
                  <th className="text-left px-5 py-3 text-xs font-medium text-gray-500 uppercase">Created</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {filtered.map((t) => (
                  <tr key={t.ticketId} className="hover:bg-gray-50 transition-colors">
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
              <div key={t.ticketId} className="px-4 py-3">
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
