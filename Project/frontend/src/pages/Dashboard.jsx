import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import { getAllTickets } from '../services/ticketService'
import { Ticket, PlusCircle, Clock } from 'lucide-react'

const STATUS_CLASSES = {
  OPEN: 'bg-emerald-100 text-emerald-800',
  CLOSED: 'bg-gray-100 text-gray-800',
  PENDING_CLOSE: 'bg-amber-100 text-amber-800',
}
const STATUS_LABELS = { OPEN: 'Otvoren', CLOSED: 'Zatvoren', PENDING_CLOSE: 'Čeka zatvaranje' }
const PRIORITY_CLASSES = {
  HIGH: 'bg-red-100 text-red-800',
  MEDIUM: 'bg-yellow-100 text-yellow-800',
  LOW: 'bg-blue-100 text-blue-800',
}
const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }

function QuickCard({ icon: Icon, label, description, to, color }) {
  const navigate = useNavigate()
  void Icon;
  return (
    <div
      onClick={() => navigate(to)}
      className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 cursor-pointer hover:shadow-md transition-shadow"
    >
      <div className="flex items-center gap-4">
        <div className={`w-12 h-12 rounded-xl flex items-center justify-center ${color}`}>
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

function RecentActivities() {
  const navigate = useNavigate()
  const [tickets, setTickets] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    getAllTickets(1, 5)
      .then((data) => setTickets(data.data))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  return (
    <div>
      <div className="flex items-center gap-2 mb-3">
        <Clock size={15} className="text-gray-400" />
        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide">
          Nedavne aktivnosti
        </h3>
      </div>
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
        {loading ? (
          <div className="flex justify-center py-8">
            <div className="w-6 h-6 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
          </div>
        ) : tickets.length === 0 ? (
          <p className="text-sm text-gray-400 text-center py-8">Nema nedavnih tiketa.</p>
        ) : (
          <ul className="divide-y divide-gray-50">
            {tickets.map((t) => (
              <li
                key={t.ticketId}
                onClick={() => navigate(`/tickets/${t.ticketId}`)}
                className="flex items-center justify-between gap-3 px-5 py-3 hover:bg-gray-50 cursor-pointer transition-colors"
              >
                <div className="min-w-0">
                  <p className="text-sm font-medium text-gray-900 truncate">{t.title}</p>
                  <p className="text-xs text-gray-400 mt-0.5">
                    {t.creatorName} &middot; {t.createdDate ? new Date(t.createdDate).toLocaleDateString('bs-BA') : '—'}
                  </p>
                </div>
                <div className="flex items-center gap-2 flex-shrink-0">
                  <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${PRIORITY_CLASSES[t.priority] ?? 'bg-gray-100 text-gray-800'}`}>
                    {PRIORITY_LABELS[t.priority] ?? t.priority}
                  </span>
                  <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${STATUS_CLASSES[t.status] ?? 'bg-gray-100 text-gray-800'}`}>
                    {STATUS_LABELS[t.status] ?? t.status}
                  </span>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}

export default function Dashboard() {
  const { user } = useAuth()

  const clientCards = [
    { icon: Ticket, label: 'Moji tiketi', description: 'Pregledajte i pratite vaše tikete za podršku', to: '/mytickets', color: 'bg-navy-600' },
    { icon: PlusCircle, label: 'Kreiraj tiket', description: 'Podnesite novi zahtjev za podršku', to: '/create-ticket', color: 'bg-emerald-500' },
  ]

  const staffCards = [
    { icon: Ticket, label: 'Tiketi', description: 'Pregledajte i upravljajte tiketima za podršku', to: '/tickets', color: 'bg-navy-600' },
  ]

  const cards = user?.role === 'CLIENT' ? clientCards : staffCards

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
        <h2 className="text-xl font-bold">
          Dobrodošli nazad, {user?.firstName}!
        </h2>
        <p className="text-navy-200 text-sm mt-1">
          {roleDescription[user?.role] || 'Dobrodošli u TelecomSupport.'}
        </p>
      </div>

      {/* Quick action cards */}
      <div>
        <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
          Brze akcije
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          {cards.map((card) => (
            <QuickCard key={card.to} {...card} />
          ))}
        </div>
      </div>

      {/* Recent activities — agents and admins only */}
      {(user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR') && (
        <RecentActivities />
      )}
    </div>
  )
}
