import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { getMyStatistics, getMyRecentTickets } from '../services/userService'
import {
  Ticket,
  PlusCircle,
  LayoutDashboard,
  BarChart2,
  CheckCircle,
  AlertCircle,
  MessageSquare,
  Clock,
  Star,
  ChevronRight,
  Loader2,
} from 'lucide-react'
import Badge from '../components/common/Badge'

// ---------- helpers ----------

function formatMinutes(minutes) {
  if (minutes == null) return '—'
  if (minutes < 60) return `${Math.round(minutes)} min`
  const h = Math.floor(minutes / 60)
  const m = Math.round(minutes % 60)
  return m > 0 ? `${h}h ${m}m` : `${h}h`
}

function formatHours(hours) {
  if (hours == null) return '—'
  if (hours < 1) return `${Math.round(hours * 60)} min`
  if (hours < 24) return `${hours.toFixed(1)}h`
  const d = Math.floor(hours / 24)
  const h = Math.round(hours % 24)
  return h > 0 ? `${d}d ${h}h` : `${d}d`
}

function formatRating(rating) {
  if (rating == null) return '—'
  return `${rating.toFixed(1)} / 5`
}

function timeAgo(dateStr) {
  const diff = Date.now() - new Date(dateStr).getTime()
  const m = Math.floor(diff / 60000)
  if (m < 1) return 'upravo'
  if (m < 60) return `prije ${m} min`
  const h = Math.floor(m / 60)
  if (h < 24) return `prije ${h}h`
  const d = Math.floor(h / 24)
  return `prije ${d}d`
}

// ---------- sub-components ----------

function QuickCard({ icon, label, description, to, color }) {
  const Icon = icon
  const navigate = useNavigate()
  return (
    <div
      onClick={() => navigate(to)}
      className="bg-white rounded-xl shadow-sm border border-gray-100 cursor-pointer hover:shadow-md transition-shadow"
    >
      {/* Mobile: compact row — icon + label + chevron */}
      <div className="flex sm:hidden items-center gap-3 px-4 py-3">
        <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${color}`}>
          <Icon size={16} className="text-white" />
        </div>
        <span className="text-sm font-semibold text-gray-900 flex-1">{label}</span>
        <ChevronRight size={14} className="text-gray-300 flex-shrink-0" />
      </div>

      {/* sm+: full card — icon + label + description */}
      <div className="hidden sm:flex items-center gap-4 p-5">
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center flex-shrink-0 ${color}`}>
          <Icon size={22} className="text-white" />
        </div>
        <div>
          <p className="text-sm font-semibold text-gray-900">{label}</p>
          <p className="text-xs text-gray-500 mt-0.5">{description}</p>
        </div>
      </div>
    </div>
  )
}

function MiniStat({ icon, label, value, color }) {
  const Icon = icon
  return (
    <div className="bg-white rounded-lg px-4 py-3 shadow-sm border border-gray-100 flex items-center gap-3">
      <div className={`w-8 h-8 rounded-lg flex items-center justify-center flex-shrink-0 ${color}`}>
        <Icon size={15} className="text-white" />
      </div>
      <div className="min-w-0">
        <p className="text-xs text-gray-400 truncate">{label}</p>
        <p className="text-sm font-bold text-gray-900 truncate">{value}</p>
      </div>
    </div>
  )
}

function RecentTicketRow({ ticket }) {
  const navigate = useNavigate()
  return (
    <div
      onClick={() => navigate(`/tickets/${ticket.ticketId}`)}
      className="flex items-center gap-3 px-4 py-3 hover:bg-gray-50 cursor-pointer transition-colors border-b border-gray-50 last:border-0"
    >
      <div className="flex-1 min-w-0">
        <p className="text-sm font-medium text-gray-900 truncate">{ticket.title}</p>
        <p className="text-xs text-gray-400 mt-0.5">{timeAgo(ticket.lastActivityDate)}</p>
      </div>
      <div className="flex items-center gap-2 flex-shrink-0">
        <Badge value={ticket.status} />
        <Badge value={ticket.priority} />
      </div>
      <ChevronRight size={14} className="text-gray-300 flex-shrink-0" />
    </div>
  )
}

// ---------- main component ----------

export default function Dashboard() {
  const { user } = useAuth()
  const isStaff = user?.role === 'AGENT' || user?.role === 'TECHNICIAN'

  const [stats, setStats] = useState(null)
  const [recentTickets, setRecentTickets] = useState([])
  const [loadingStats, setLoadingStats] = useState(isStaff)
  const [loadingTickets, setLoadingTickets] = useState(isStaff)

  useEffect(() => {
    if (!isStaff) return

    getMyStatistics()
      .then(setStats)
      .catch(() => {})
      .finally(() => setLoadingStats(false))

    getMyRecentTickets()
      .then(setRecentTickets)
      .catch(() => {})
      .finally(() => setLoadingTickets(false))
  }, [isStaff])

  // Quick action cards
  const clientCards = [
    { icon: Ticket, label: 'Moji tiketi', description: 'Pregledajte i pratite vaše tikete', to: '/mytickets', color: 'bg-navy-600' },
    { icon: PlusCircle, label: 'Kreiraj tiket', description: 'Podnesite novi zahtjev za podršku', to: '/create-ticket', color: 'bg-emerald-500' },
  ]

  const agentCards = [
    { icon: Ticket, label: 'Svi tiketi', description: 'Pregledajte i upravljajte svim tiketima', to: '/tickets', color: 'bg-navy-600' },
    { icon: Ticket, label: 'Dodijeljeni meni', description: 'Tiketi dodijeljeni vama', to: '/assigned', color: 'bg-blue-500' },
    { icon: BarChart2, label: 'Moja statistika', description: 'Detaljan pregled vaših performansi', to: '/statistics', color: 'bg-violet-500' },
  ]

  const techCards = [
    { icon: Ticket, label: 'Dodijeljeni meni', description: 'Tiketi dodijeljeni vama', to: '/assigned', color: 'bg-navy-600' },
    { icon: BarChart2, label: 'Moja statistika', description: 'Detaljan pregled vaših performansi', to: '/statistics', color: 'bg-violet-500' },
  ]

  const adminCards = [
    { icon: Ticket, label: 'Svi tiketi', description: 'Pregledajte i upravljajte tiketima', to: '/tickets', color: 'bg-navy-600' },
    { icon: LayoutDashboard, label: 'FAQ', description: 'Upravljajte često postavljanim pitanjima', to: '/faq', color: 'bg-emerald-500' },
  ]

  const cards = {
    CLIENT: clientCards,
    AGENT: agentCards,
    TECHNICIAN: techCards,
    ADMINISTRATOR: adminCards,
  }[user?.role] ?? []

  // Condensed stats config
  const miniStats = stats ? [
    { icon: Ticket,        label: 'Otvoreni',         value: stats.openTicketsCount,                    color: 'bg-blue-500' },
    { icon: CheckCircle,   label: 'Zatvoreni',         value: stats.closedTicketsCount,                  color: 'bg-emerald-500' },
    { icon: AlertCircle,   label: 'Čeka zatvaranje',   value: stats.pendingClosureCount,                 color: 'bg-amber-500' },
    { icon: MessageSquare, label: 'Prosj. 1. odgovor', value: formatMinutes(stats.avgFirstResponseMinutes), color: 'bg-violet-500' },
    { icon: Clock,         label: 'Prosj. rješavanje', value: formatHours(stats.avgResolutionHours),     color: 'bg-navy-600' },
    ...(user?.role === 'AGENT'
      ? [{ icon: Star, label: 'Prosj. ocjena', value: formatRating(stats.avgRating), color: 'bg-yellow-500' }]
      : []),
  ] : []

  const roleDescription = {
    CLIENT: 'Ovdje je pregled vašeg korisničkog računa za podršku.',
    AGENT: 'Ovdje su vaši tiketi na čekanju i sažetak aktivnosti.',
    TECHNICIAN: 'Ovdje su vaši dodijeljeni tiketi i zadaci.',
    ADMINISTRATOR: 'Ovdje je vaš sistemski pregled.',
  }

  return (
    <div className="space-y-6">
      {/* Welcome banner */}
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl p-6 text-white">
        <h2 className="text-xl font-bold">Dobrodošli nazad, {user?.firstName}!</h2>
        <p className="text-navy-200 text-sm mt-1">
          {roleDescription[user?.role] ?? 'Dobrodošli u TelecomSupport.'}
        </p>
      </div>

      {/* Quick actions */}
      <div>
        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">Brze akcije</h3>
        <div className="grid grid-cols-1 gap-2 sm:grid-cols-2 sm:gap-4 lg:grid-cols-3">
          {cards.map((card) => (
            <QuickCard key={card.to} {...card} />
          ))}
        </div>
      </div>

      {/* Stats + Recent tickets — samo za AGENT i TECHNICIAN */}
      {isStaff && (
        <>
          {/* Condensed stats */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">Moja statistika</h3>
            {loadingStats ? (
              <div className="flex items-center gap-2 text-gray-400 text-sm py-2">
                <Loader2 size={16} className="animate-spin" /> Učitavanje...
              </div>
            ) : (
              <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-6 gap-3">
                {miniStats.map((s) => (
                  <MiniStat key={s.label} {...s} />
                ))}
              </div>
            )}
          </div>

          {/* Recently updated tickets */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">Nedavna aktivnost</h3>
            {loadingTickets ? (
              <div className="flex items-center gap-2 text-gray-400 text-sm py-2">
                <Loader2 size={16} className="animate-spin" /> Učitavanje...
              </div>
            ) : recentTickets.length === 0 ? (
              <div className="bg-white rounded-xl border border-gray-100 shadow-sm px-4 py-6 text-center text-sm text-gray-400">
                Nema dodijeljenih tiketa.
              </div>
            ) : (
              <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
                {recentTickets.map((t) => (
                  <RecentTicketRow key={t.ticketId} ticket={t} />
                ))}
              </div>
            )}
          </div>
        </>
      )}
    </div>
  )
}
