import { useState, useEffect, useMemo } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { getMyTickets } from '../services/ticketService'
import { Search, Plus, Ticket } from 'lucide-react'
import { formatDateOnly } from '../utils/formatDate'
import EmptyState from '../components/common/EmptyState'
import Badge from '../components/common/Badge'

const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }
const STATUS_LABELS = { OPEN: 'Otvoren', CLOSED: 'Zatvoren', CLOSURE_REQUESTED: 'Čeka zatvaranje' }
const TYPE_LABELS = {
    INTERNET: 'Internet',
    TV: 'TV',
    MOBILE_NETWORK: 'Mobilna mreža',
    BILLING: 'Računi/Naplata',
    TECHNICAL_SUPPORT: 'Tehnička podrška',
}

const EMPTY_FILTERS = { priority: '', status: '', type: '', dateFrom: '' }

function SkeletonTable() {
    return (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden animate-pulse">
            <div className="hidden md:block">
                <div className="flex gap-4 px-5 py-3 border-b border-gray-100 bg-gray-50">
                    {[180, 80, 70, 80, 80].map((w, i) => (
                        <div key={i} className="h-3 bg-gray-200 rounded" style={{ width: w }} />
                    ))}
                </div>
                {Array.from({ length: 5 }).map((_, i) => (
                    <div key={i} className="flex items-center gap-4 px-5 py-3.5 border-b border-gray-50 last:border-0">
                        <div className="flex-1">
                            <div className="h-3.5 bg-gray-200 rounded w-48 mb-1.5" />
                            <div className="h-2.5 bg-gray-100 rounded w-16" />
                        </div>
                        <div className="h-5 bg-gray-200 rounded-full w-20" />
                        <div className="h-5 bg-gray-200 rounded-full w-14" />
                        <div className="h-3.5 bg-gray-100 rounded w-20" />
                        <div className="h-3 bg-gray-100 rounded w-20" />
                    </div>
                ))}
            </div>
            <div className="md:hidden divide-y divide-gray-50">
                {Array.from({ length: 4 }).map((_, i) => (
                    <div key={i} className="px-4 py-3">
                        <div className="h-3.5 bg-gray-200 rounded w-48 mb-2" />
                        <div className="flex gap-2">
                            <div className="h-5 bg-gray-200 rounded-full w-16" />
                            <div className="h-5 bg-gray-200 rounded-full w-14" />
                        </div>
                    </div>
                ))}
            </div>
        </div>
    )
}

export default function MyTickets() {
    const navigate = useNavigate()
    const [tickets, setTickets] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [filters, setFilters] = useState(EMPTY_FILTERS)
    const [search, setSearch] = useState('')

    useEffect(() => {
        getMyTickets()
            .then(setTickets)
            .catch((err) => {
                console.error(err)
                setError('Greška pri učitavanju tiketa.')
            })
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

    const removeFilter = (key) => {
        setFilters((f) => ({ ...f, [key]: '' }))
    }

    const filterLabel = (key, value) => {
        if (key === 'priority') return `Prioritet: ${PRIORITY_LABELS[value] ?? value}`
        if (key === 'status') return `Status: ${STATUS_LABELS[value] ?? value}`
        if (key === 'type') return `Tip: ${TYPE_LABELS[value] ?? value}`
        if (key === 'dateFrom') return `Od: ${formatDateOnly(value)}`
        return value
    }

    const openTicketDetails = (ticketId) => {
        navigate(`/mytickets/${ticketId}`)
    }

    return (
        <div className="space-y-4">
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
                        className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
                    >
                        <option value="">Svi prioriteti</option>
                        {Object.entries(PRIORITY_LABELS).map(([val, label]) => (
                            <option key={val} value={val}>{label}</option>
                        ))}
                    </select>

                    <select
                        value={filters.status}
                        onChange={(e) => setFilters((f) => ({ ...f, status: e.target.value }))}
                        className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
                    >
                        <option value="">Svi statusi</option>
                        {Object.entries(STATUS_LABELS).map(([val, label]) => (
                            <option key={val} value={val}>{label}</option>
                        ))}
                    </select>

                    <select
                        value={filters.type}
                        onChange={(e) => setFilters((f) => ({ ...f, type: e.target.value }))}
                        className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
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
                        className="px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500"
                    />

                    <Link to="/create-ticket">
                        <button className="flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-800 text-white font-medium rounded-lg text-sm transition-colors whitespace-nowrap">
                            <Plus size={16} />
                            Novi tiket
                        </button>
                    </Link>
                </div>
            </div>

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
                                className="text-navy-400 hover:text-navy-700 leading-none text-sm"
                                aria-label="Ukloni filter"
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

            {loading ? (
                <SkeletonTable />
            ) : error ? (
                <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
                    {error}
                </div>
            ) : filteredTickets.length === 0 ? (
                <EmptyState
                    icon={Ticket}
                    title={tickets.length === 0 ? 'Još nema tiketa' : 'Nijedan tiket ne odgovara filterima'}
                    description={
                        tickets.length === 0
                            ? 'Kreirajte vaš prvi tiket za podršku da biste započeli.'
                            : 'Pokušajte prilagoditi ili očistiti filtere.'
                    }
                    action={tickets.length === 0 ? () => navigate('/create-ticket') : undefined}
                    actionLabel={tickets.length === 0 ? 'Kreiraj tiket' : undefined}
                />
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
                                {filteredTickets.map((ticket) => (
                                    <tr
                                        key={ticket.ticketId}
                                        onClick={() => openTicketDetails(ticket.ticketId)}
                                        className="hover:bg-gray-50 transition-colors cursor-pointer"
                                    >
                                        <td className="px-5 py-3">
                                            <p className="text-sm font-medium text-gray-900 truncate max-w-xs">
                                                {ticket.title || ticket.subject}
                                            </p>
                                            <p className="text-xs text-gray-400">{ticket.ticketId}</p>
                                        </td>

                                        <td className="px-5 py-3">
                                            <Badge value={ticket.status} />
                                        </td>

                                        <td className="px-5 py-3">
                                            <Badge value={ticket.priority} />
                                        </td>

                                        <td className="px-5 py-3 text-sm text-gray-600">
                                            {TYPE_LABELS[ticket.problemCategory] ?? ticket.problemCategory}
                                        </td>

                                        <td className="px-5 py-3 text-sm text-gray-500 whitespace-nowrap">
                                            {ticket.createdDate
                                                ? formatDateOnly(ticket.createdDate)
                                                : '—'}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>

                    <div className="md:hidden divide-y divide-gray-50">
                        {filteredTickets.map((ticket) => (
                            <div
                                key={ticket.ticketId}
                                onClick={() => openTicketDetails(ticket.ticketId)}
                                className="px-4 py-3 cursor-pointer hover:bg-gray-50"
                            >
                                <p className="text-sm font-medium text-gray-900 truncate">
                                    {ticket.title || ticket.subject}
                                </p>

                                <div className="flex flex-wrap items-center gap-2 mt-2">
                                    <Badge value={ticket.status} />
                                    <Badge value={ticket.priority} />
                                </div>

                                <p className="text-xs text-gray-400 mt-1">
                                    {ticket.createdDate
                                        ? new Date(ticket.createdDate).toLocaleDateString('bs-BA')
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
