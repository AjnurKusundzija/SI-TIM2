import PropTypes from 'prop-types'
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
import {
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Tooltip,
  Legend,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
} from 'recharts'

function StatCard({ icon, label, value, color, description }) {
  const Icon = icon
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

StatCard.propTypes = {
  icon: PropTypes.elementType.isRequired,
  label: PropTypes.string.isRequired,
  value: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  color: PropTypes.string.isRequired,
  description: PropTypes.string,
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

const DISTRIBUTION_COLORS = {
  open: '#3b82f6',
  closed: '#10b981',
  pending: '#f59e0b',
}

const CustomTooltip = ({ active, payload }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white border border-gray-200 rounded-lg px-3 py-2 shadow-sm text-xs">
      <p className="font-semibold text-gray-800">{payload[0].name}</p>
      <p className="text-gray-600">{payload[0].value} tiketa</p>
    </div>
  )
}

const CustomBarTooltip = ({ active, payload, label }) => {
  if (!active || !payload?.length) return null
  return (
    <div className="bg-white border border-gray-200 rounded-lg px-3 py-2 shadow-sm text-xs">
      <p className="font-semibold text-gray-800 mb-1">{label}</p>
      {payload.map((p) => (
        <p key={p.name} className="text-gray-600">{p.value}</p>
      ))}
    </div>
  )
}

CustomTooltip.propTypes = {
  active: PropTypes.bool,
  payload: PropTypes.array,
}

CustomBarTooltip.propTypes = {
  active: PropTypes.bool,
  payload: PropTypes.array,
  label: PropTypes.string,
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

  const statCards = [
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
    statCards.push({
      icon: Star,
      label: 'Prosječna ocjena',
      value: formatRating(stats.avgRating),
      color: 'bg-yellow-500',
      description: 'Ocjena korisnika na zatvorenim tiketima',
    })
  }

  const totalTickets = stats.openTicketsCount + stats.closedTicketsCount + stats.pendingClosureCount

  const distributionData = [
    { name: 'Otvoreni', value: stats.openTicketsCount, color: DISTRIBUTION_COLORS.open },
    { name: 'Zatvoreni', value: stats.closedTicketsCount, color: DISTRIBUTION_COLORS.closed },
    { name: 'Čeka zatvaranje', value: stats.pendingClosureCount, color: DISTRIBUTION_COLORS.pending },
  ].filter((d) => d.value > 0)

  // Performance bar chart data — in minutes for comparison
  const perfData = [
    {
      name: '1. odgovor',
      vrijednost: stats.avgFirstResponseMinutes != null ? Math.round(stats.avgFirstResponseMinutes) : null,
      label: formatMinutes(stats.avgFirstResponseMinutes) ?? '—',
    },
    {
      name: 'Rješavanje',
      vrijednost: stats.avgResolutionHours != null ? Math.round(stats.avgResolutionHours * 60) : null,
      label: formatHours(stats.avgResolutionHours) ?? '—',
    },
  ].filter((d) => d.vrijednost != null)

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-xl p-6 text-white">
        <h2 className="text-xl font-bold">Moja statistika</h2>
        <p className="text-navy-200 text-sm mt-1">
          Pregled vašeg rada i performansi unutar sistema
        </p>
      </div>

      {totalTickets === 0 ? (
        <div className="bg-white rounded-xl p-8 shadow-sm border border-gray-100 text-center">
          <Ticket size={36} className="mx-auto text-gray-300 mb-3" />
          <p className="text-gray-500 text-sm font-medium">Nema dodijeljenih tiketa</p>
          <p className="text-gray-400 text-xs mt-1">Statistika će se prikazati kada dobijete dodijeljene tikete.</p>
        </div>
      ) : (
        <>
          {/* Opterećenje — stat cards + donut chart */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Opterećenje
            </h3>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
              <div className="lg:col-span-2 grid grid-cols-1 sm:grid-cols-3 gap-4 content-start">
                {statCards.slice(0, 3).map((card) => (
                  <StatCard key={card.label} {...card} />
                ))}
              </div>

              {/* Donut chart */}
              <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100 flex flex-col items-center justify-center">
                <p className="text-sm font-semibold text-gray-700 mb-3">Distribucija tiketa</p>
                <ResponsiveContainer width="100%" height={180}>
                  <PieChart>
                    <Pie
                      data={distributionData}
                      cx="50%"
                      cy="50%"
                      innerRadius={48}
                      outerRadius={72}
                      paddingAngle={3}
                      dataKey="value"
                    >
                      {distributionData.map((entry) => (
                        <Cell key={entry.name} fill={entry.color} />
                      ))}
                    </Pie>
                    <Tooltip content={<CustomTooltip />} />
                    <Legend
                      iconType="circle"
                      iconSize={8}
                      formatter={(value) => <span className="text-xs text-gray-600">{value}</span>}
                    />
                  </PieChart>
                </ResponsiveContainer>
                <p className="text-xs text-gray-400 mt-1">Ukupno: {totalTickets}</p>
              </div>
            </div>
          </div>

          {/* Performanse — stat cards + bar chart */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-wide mb-3">
              Performanse
            </h3>
            <div className="grid grid-cols-1 lg:grid-cols-3 gap-4">
              <div className="lg:col-span-2 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-2 gap-4 content-start">
                {statCards.slice(3).map((card) => (
                  <StatCard key={card.label} {...card} />
                ))}
              </div>

              {/* Bar chart — response times in minutes */}
              {perfData.length > 0 && (
                <div className="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                  <p className="text-sm font-semibold text-gray-700 mb-3">Vremena odgovora (min)</p>
                  <ResponsiveContainer width="100%" height={160}>
                    <BarChart data={perfData} barCategoryGap="40%">
                      <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" vertical={false} />
                      <XAxis dataKey="name" tick={{ fontSize: 11, fill: '#9ca3af' }} axisLine={false} tickLine={false} />
                      <YAxis tick={{ fontSize: 11, fill: '#9ca3af' }} axisLine={false} tickLine={false} width={36} />
                      <Tooltip content={<CustomBarTooltip />} cursor={{ fill: '#f3f4f6' }} />
                      <Bar dataKey="vrijednost" fill="#8b5cf6" radius={[4, 4, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                  <p className="text-xs text-gray-400 mt-2 text-center">Vrijednosti u minutama</p>
                </div>
              )}
            </div>
          </div>
        </>
      )}
    </div>
  )
}
