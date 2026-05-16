import { useState, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'
import { getMyStatistics } from '../services/userService'
import {
  Ticket,
  CheckCircle,
  Clock,
  MessageSquare,
  Star,
  AlertCircle,
  Loader2,
} from 'lucide-react'

function StatCard({ icon: Icon, label, value, color, description }) {
  return (
    <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-gray-500 mb-1">{label}</p>
          <p className="text-2xl font-bold text-gray-900">
            {value ?? <span className="text-gray-400 text-lg">—</span>}
          </p>
          {description && (
            <p className="text-xs text-gray-400 mt-1">{description}</p>
          )}
        </div>
        <div className={`w-11 h-11 rounded-xl flex items-center justify-center ${color}`}>
          <Icon size={20} className="text-white" />
        </div>
      </div>
    </div>
  )
}

function formatMinutes(minutes) {
  if (minutes == null) return null
  if (minutes < 60) return `${Math.round(minutes)} min`
  const h = Math.floor(minutes / 60)
  const m = Math.round(minutes % 60)
  return m > 0 ? `${h}h ${m}min` : `${h}h`
}

function formatHours(hours) {
  if (hours == null) return null
  if (hours < 1) return `${Math.round(hours * 60)} min`
  if (hours < 24) return `${hours.toFixed(1)}h`
  const days = Math.floor(hours / 24)
  const h = Math.round(hours % 24)
  return h > 0 ? `${days}d ${h}h` : `${days}d`
}

function formatRating(rating) {
  if (rating == null) return null
  return `${rating.toFixed(1)} / 5`
}

export default function Statistics() {
  const { user } = useAuth()
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    getMyStatistics()
      .then((data) => { setStats(data); setError(null) })
      .catch(() => setError('Greška pri učitavanju statistike.'))
      .finally(() => setLoading(false))
  }, [])

  if (loading) {
    return (
      <div className="flex items-center justify-center h-48 text-gray-400">
        <Loader2 size={24} className="animate-spin mr-2" />
        <span className="text-sm">Učitavanje statistike...</span>
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex items-center gap-2 text-red-500 bg-red-50 border border-red-100 rounded-xl p-4">
        <AlertCircle size={18} />
        <span className="text-sm">{error}</span>
      </div>
    )
  }

  const cards = [
    {
      icon: Ticket,
      label: 'Otvoreni tiketi',
      value: stats.openTicketsCount,
      color: 'bg-blue-500',
      description: 'Trenutno aktivni tiketi',
    },
    {
      icon: CheckCircle,
      label: 'Zatvoreni tiketi',
      value: stats.closedTicketsCount,
      color: 'bg-emerald-500',
      description: 'Uspješno riješeni tiketi',
    },
    {
      icon: AlertCircle,
      label: 'Čeka zatvaranje',
      value: stats.pendingClosureCount,
      color: 'bg-amber-500',
      description: 'Tiketi na čekanju potvrde',
    },
    {
      icon: MessageSquare,
      label: 'Prosj. prvi odgovor',
      value: formatMinutes(stats.avgFirstResponseMinutes),
      color: 'bg-violet-500',
      description: 'Prosječno vrijeme do prvog odgovora',
    },
    {
      icon: Clock,
      label: 'Prosj. rješavanje',
      value: formatHours(stats.avgResolutionHours),
      color: 'bg-navy-600',
      description: 'Prosječno vrijeme od otvaranja do zatvaranja',
    },
  ]

  if (user?.role === 'AGENT') {
    cards.push({
      icon: Star,
      label: 'Prosječna ocjena',
      value: formatRating(stats.avgRating),
      color: 'bg-yellow-500',
      description: 'Ocjena korisnika na zatvorenim tiketima',
    })
  }

  const totalTickets = stats.openTicketsCount + stats.closedTicketsCount + stats.pendingClosureCount

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl p-6 text-white">
        <h2 className="text-xl font-bold">Moja statistika</h2>
        <p className="text-navy-200 text-sm mt-1">
          Pregled vašeg rada i performansi unutar sistema
        </p>
      </div>

      {/* Ukupno tiketa — summary */}
      {totalTickets === 0 ? (
        <div className="bg-white rounded-xl p-8 shadow-sm border border-gray-100 text-center">
          <Ticket size={36} className="mx-auto text-gray-300 mb-3" />
          <p className="text-gray-500 text-sm">Nema dodijeljenih tiketa za prikaz statistike.</p>
        </div>
      ) : (
        <>
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Opterećenje
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
              {cards.slice(0, 3).map((card) => (
                <StatCard key={card.label} {...card} />
              ))}
            </div>
          </div>

          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Performanse
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
              {cards.slice(3).map((card) => (
                <StatCard key={card.label} {...card} />
              ))}
            </div>
          </div>
        </>
      )}
    </div>
  )
}
