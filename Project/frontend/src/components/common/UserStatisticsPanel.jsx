import PropTypes from 'prop-types'
import { useState, useEffect } from 'react'
import { getUserStatistics } from '../../services/userService'
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

export default function UserStatisticsPanel({ userId, role }) {
  const [stats, setStats] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    if (!userId) return

    const t = setTimeout(() => setLoading(true), 0)
    getUserStatistics(userId)
      .then((data) => { setStats(data); setError(null) })
      .catch(() => setError('Greška pri učitavanju statistike korisnika.'))
      .finally(() => setLoading(false))
      
    return () => clearTimeout(t)
  }, [userId])

  if (loading) {
    return (
      <div className="flex items-center justify-center h-48 text-gray-400 bg-white rounded-3xl border border-slate-200 w-full mt-8">
        <Loader2 size={24} className="animate-spin mr-2" />
        <span className="text-sm">Učitavanje statistike...</span>
      </div>
    )
  }

  if (error) {
    return (
      <div className="flex items-center justify-center gap-2 text-red-500 bg-red-50 border border-red-100 rounded-3xl p-8 mt-8">
        <AlertCircle size={20} />
        <span className="text-sm font-medium">{error}</span>
      </div>
    )
  }

  if (!stats) return null

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
      label: 'Čeka se',
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

  if (role === 'AGENT') {
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
    { name: 'Čeka se', value: stats.pendingClosureCount, color: DISTRIBUTION_COLORS.pending },
  ].filter((d) => d.value > 0)

  // Performance bar chart data
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
    <section className="space-y-6 mt-8">
      {/* Header */}
      <div className="bg-gradient-to-r from-navy-800 to-navy-700 rounded-3xl p-8 text-white shadow-sm">
        <h2 className="text-xl font-bold">Statistika rada</h2>
        <p className="text-navy-200 text-sm mt-1">
          Pregled rada i performansi odabranog uposlenika
        </p>
      </div>

      {totalTickets === 0 ? (
        <div className="bg-white rounded-3xl p-10 shadow-sm border border-slate-200 text-center">
          <Ticket size={40} className="mx-auto text-gray-300 mb-4" />
          <p className="text-gray-500 text-base font-medium">Nema dodijeljenih tiketa</p>
          <p className="text-gray-400 text-sm mt-1">Ovaj uposlenik još uvijek nema statističkih podataka.</p>
        </div>
      ) : (
        <>
          {/* Stat cards */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-[0.2em] mb-4">Pregled</h3>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
              {statCards.map((card) => (
                <StatCard key={card.label} {...card} />
              ))}
            </div>
          </div>

          {/* Charts */}
          <div>
            <h3 className="text-sm font-semibold text-gray-500 uppercase tracking-[0.2em] mb-4">Grafikoni performansi</h3>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
              {/* Donut chart */}
              <div className="bg-white rounded-3xl p-6 shadow-sm border border-slate-200">
                <p className="text-sm font-semibold text-gray-700 mb-1">Distribucija tiketa</p>
                <p className="text-xs text-gray-400 mb-5">Ukupno: {totalTickets}</p>
                <ResponsiveContainer width="100%" height={220}>
                  <PieChart>
                    <Pie
                      data={distributionData}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={90}
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
              </div>

              {/* Bar chart */}
              {perfData.length > 0 ? (
                <div className="bg-white rounded-3xl p-6 shadow-sm border border-slate-200">
                  <p className="text-sm font-semibold text-gray-700 mb-1">Vremena odgovora</p>
                  <p className="text-xs text-gray-400 mb-5">Vrijednosti u minutama</p>
                  <ResponsiveContainer width="100%" height={220}>
                    <BarChart data={perfData} barCategoryGap="40%">
                      <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" vertical={false} />
                      <XAxis dataKey="name" tick={{ fontSize: 12, fill: '#9ca3af' }} axisLine={false} tickLine={false} />
                      <YAxis tick={{ fontSize: 12, fill: '#9ca3af' }} axisLine={false} tickLine={false} width={40} />
                      <Tooltip content={<CustomBarTooltip />} cursor={{ fill: '#f3f4f6' }} />
                      <Bar dataKey="vrijednost" fill="#8b5cf6" radius={[6, 6, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              ) : (
                <div className="bg-white rounded-3xl p-6 shadow-sm border border-slate-200 flex items-center justify-center text-sm text-gray-400">
                  Nema dovoljno podataka za prikaz grafikona.
                </div>
              )}
            </div>
          </div>
        </>
      )}
    </section>
  )
}

UserStatisticsPanel.propTypes = {
  userId: PropTypes.number.isRequired,
  role: PropTypes.string.isRequired,
}
