import { useState, useEffect, useMemo } from 'react'
import { useNavigate } from 'react-router-dom'
import { Users, Search, ChevronUp, ChevronDown, Filter, RefreshCw, ArrowRightLeft, X, Check, AlertTriangle, Wifi, Tv, Smartphone, CreditCard, Wrench, HardDrive } from 'lucide-react'
import { getTeamsOverview, reassignAgent } from '../../services/teamService'

// ─── Category helpers ────────────────────────────────────────────────────────

const CATEGORY_META = {
  INTERNET: { label: 'Internet', color: 'bg-blue-100 text-blue-700 border-blue-200', icon: Wifi },
  TV: { label: 'TV', color: 'bg-purple-100 text-purple-700 border-purple-200', icon: Tv },
  MOBILE_NETWORK: { label: 'Mobilna mreža', color: 'bg-emerald-100 text-emerald-700 border-emerald-200', icon: Smartphone },
  BILLING: { label: 'Naplata', color: 'bg-amber-100 text-amber-700 border-amber-200', icon: CreditCard },
  TECHNICAL_SUPPORT: { label: 'Tehnička podrška', color: 'bg-rose-100 text-rose-700 border-rose-200', icon: Wrench },
  '': { label: 'Opšti', color: 'bg-slate-100 text-slate-600 border-slate-200', icon: HardDrive },
}

function getCategoryMeta(key) {
  return CATEGORY_META[key] || CATEGORY_META['']
}

// ─── Availability badge ───────────────────────────────────────────────────────

function AvailabilityBadge({ status }) {
  const map = {
    AVAILABLE: 'bg-emerald-100 text-emerald-700',
    UNAVAILABLE: 'bg-red-100 text-red-600',
    ON_BREAK: 'bg-amber-100 text-amber-600',
  }
  const label = { AVAILABLE: 'Dostupan', UNAVAILABLE: 'Nedostupan', ON_BREAK: 'Pauza' }
  return (
    <span className={`inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[10px] font-medium ${map[status] || 'bg-slate-100 text-slate-500'}`}>
      <span className={`w-1.5 h-1.5 rounded-full ${status === 'AVAILABLE' ? 'bg-emerald-500' : status === 'ON_BREAK' ? 'bg-amber-500' : 'bg-red-500'}`} />
      {label[status] ?? status}
    </span>
  )
}

// ─── Reassign Modal ───────────────────────────────────────────────────────────

function ReassignModal({ agent, currentTeam, allTeams, onConfirm, onClose, loading, error }) {
  const [selectedTeamId, setSelectedTeamId] = useState('')

  const availableTeams = allTeams.filter(t => t.teamId !== currentTeam.teamId)

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div className="absolute inset-0 bg-black/50 backdrop-blur-sm" onClick={onClose} />
      <div className="relative bg-white rounded-2xl shadow-2xl w-full max-w-md p-6 flex flex-col gap-5">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-9 h-9 rounded-xl bg-navy-100 flex items-center justify-center">
              <ArrowRightLeft size={17} className="text-navy-700" />
            </div>
            <div>
              <h2 className="text-base font-semibold text-gray-900">Premjesti agenta</h2>
              <p className="text-xs text-gray-400 mt-0.5">Odaberi odredišni tim</p>
            </div>
          </div>
          <button onClick={onClose} className="p-1.5 rounded-lg hover:bg-slate-100 transition-colors">
            <X size={16} className="text-gray-400" />
          </button>
        </div>

        {/* Agent info */}
        <div className="bg-slate-50 rounded-xl p-4 flex items-center gap-3">
          <div className="w-10 h-10 rounded-full bg-navy-700 flex items-center justify-center text-sm font-semibold text-white flex-shrink-0">
            {agent.firstName[0]}{agent.lastName[0]}
          </div>
          <div className="min-w-0">
            <p className="text-sm font-semibold text-gray-900">{agent.firstName} {agent.lastName}</p>
            <p className="text-xs text-gray-400">Trenutni tim: <span className="font-medium text-gray-600">{currentTeam.teamName}</span></p>
          </div>
        </div>

        {/* Destination team select */}
        <div>
          <label className="block text-xs font-semibold text-gray-700 mb-1.5">Odredišni tim</label>
          <select
            id="reassign-team-select"
            value={selectedTeamId}
            onChange={e => setSelectedTeamId(e.target.value)}
            className="w-full rounded-lg border border-slate-200 px-3 py-2.5 text-sm text-gray-800 focus:outline-none focus:ring-2 focus:ring-navy-300 focus:border-navy-400 bg-white"
          >
            <option value="">— Odaberi tim —</option>
            {availableTeams.map(t => (
              <option key={t.teamId} value={t.teamId}>
                {t.teamName} {t.specializedCategory ? `(${getCategoryMeta(t.specializedCategory).label})` : ''}
              </option>
            ))}
          </select>
        </div>

        {/* Error */}
        {error && (
          <div className="flex items-start gap-2.5 p-3 rounded-xl bg-red-50 border border-red-200">
            <AlertTriangle size={15} className="text-red-500 flex-shrink-0 mt-0.5" />
            <p className="text-sm text-red-700">{error}</p>
          </div>
        )}

        {/* Actions */}
        <div className="flex gap-3 pt-1">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-2.5 rounded-xl border border-slate-200 text-sm font-medium text-gray-700 hover:bg-slate-50 transition-colors"
          >
            Odustani
          </button>
          <button
            id="reassign-confirm-btn"
            onClick={() => onConfirm(agent.userId, parseInt(selectedTeamId))}
            disabled={!selectedTeamId || loading}
            className="flex-1 px-4 py-2.5 rounded-xl bg-navy-700 text-white text-sm font-medium hover:bg-navy-800 disabled:opacity-50 disabled:cursor-not-allowed transition-colors flex items-center justify-center gap-2"
            style={{ backgroundColor: selectedTeamId && !loading ? '#1e3a5f' : undefined }}
          >
            {loading ? (
              <span className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
            ) : (
              <Check size={15} />
            )}
            Premjesti
          </button>
        </div>
      </div>
    </div>
  )
}

// ─── Agent row ────────────────────────────────────────────────────────────────

function AgentRow({ member, team, onReassign }) {
  const navigate = useNavigate()
  
  return (
    <div className="flex items-center gap-3 px-4 py-2.5 hover:bg-slate-50 transition-colors rounded-lg group">
      <div 
        className="w-8 h-8 rounded-full bg-gradient-to-br from-navy-100 to-navy-200 flex items-center justify-center text-xs font-semibold text-navy-700 flex-shrink-0 cursor-pointer"
        onClick={() => navigate(`/users/${member.userId}`)}
      >
        {member.firstName[0]}{member.lastName[0]}
      </div>
      <div className="flex-1 min-w-0">
        <p 
          className="text-sm font-medium text-gray-900 truncate cursor-pointer hover:underline hover:text-navy-700"
          onClick={() => navigate(`/users/${member.userId}`)}
        >
          {member.firstName} {member.lastName}
        </p>
        <div className="flex items-center gap-2 mt-0.5">
          <AvailabilityBadge status={member.availability} />
          {member.openTicketCount > 0 && (
            <span className="text-[10px] text-amber-600 font-medium bg-amber-50 px-1.5 py-0.5 rounded border border-amber-100">
              {member.openTicketCount} otvorenih
            </span>
          )}
        </div>
      </div>
      <button
        onClick={() => onReassign(member, team)}
        className="opacity-0 group-hover:opacity-100 flex items-center gap-1.5 px-2.5 py-1.5 rounded-lg text-xs font-medium text-navy-700 bg-navy-50 hover:bg-navy-100 border border-navy-100 transition-all"
        title="Premjesti agenta"
      >
        <ArrowRightLeft size={12} />
        Premjesti
      </button>
    </div>
  )
}

// ─── Team Card ────────────────────────────────────────────────────────────────

function TeamCard({ team, onReassign }) {
  const [expanded, setExpanded] = useState(true)
  const meta = getCategoryMeta(team.specializedCategory)
  const Icon = meta.icon

  return (
    <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
      {/* Card header */}
      <div
        className="flex items-center gap-3 px-5 py-4 cursor-pointer select-none hover:bg-slate-50/60 transition-colors"
        onClick={() => setExpanded(e => !e)}
      >
        <div className={`w-9 h-9 rounded-xl border flex items-center justify-center flex-shrink-0 ${meta.color}`}>
          <Icon size={16} />
        </div>
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 flex-wrap">
            <h3 className="text-sm font-semibold text-gray-900">{team.teamName}</h3>
            <span className={`px-2 py-0.5 rounded-full border text-[10px] font-semibold ${meta.color}`}>
              {meta.label}
            </span>
          </div>
        </div>

        {/* Stats */}
        <div className="flex items-center gap-4 text-center mr-2">
          <div>
            <p className="text-lg font-bold text-navy-800 leading-tight">{team.activeAgentCount}</p>
            <p className="text-[10px] text-gray-400 leading-tight">Agenti</p>
          </div>
          <div className="w-px h-8 bg-slate-200" />
          <div>
            <p className={`text-lg font-bold leading-tight ${team.openTicketCount > 0 ? 'text-amber-600' : 'text-emerald-600'}`}>
              {team.openTicketCount}
            </p>
            <p className="text-[10px] text-gray-400 leading-tight">Otvoreni</p>
          </div>
        </div>

        <span className="text-gray-300 ml-1">
          {expanded ? <ChevronUp size={16} /> : <ChevronDown size={16} />}
        </span>
      </div>

      {/* Member list */}
      {expanded && (
        <div className="border-t border-slate-100">
          {team.members.length === 0 ? (
            <p className="px-5 py-4 text-sm text-gray-400 italic">Nema aktivnih članova u ovom timu.</p>
          ) : (
            <div className="px-2 py-2 space-y-0.5">
              {team.members.map(member => (
                <AgentRow
                  key={member.userId}
                  member={member}
                  team={team}
                  onReassign={onReassign}
                />
              ))}
            </div>
          )}
        </div>
      )}
    </div>
  )
}

// ─── Toast ────────────────────────────────────────────────────────────────────

function Toast({ message, type, onClose }) {
  useEffect(() => {
    const t = setTimeout(onClose, 4000)
    return () => clearTimeout(t)
  }, [onClose])

  const styles = {
    success: 'bg-emerald-50 border-emerald-200 text-emerald-800',
    error: 'bg-red-50 border-red-200 text-red-800',
  }

  return (
    <div className={`fixed bottom-6 right-6 z-50 flex items-center gap-3 px-4 py-3 rounded-xl border shadow-lg max-w-sm animate-slideUp ${styles[type]}`}>
      {type === 'success' ? <Check size={15} className="flex-shrink-0" /> : <AlertTriangle size={15} className="flex-shrink-0" />}
      <p className="text-sm font-medium">{message}</p>
      <button onClick={onClose} className="ml-2 opacity-60 hover:opacity-100">
        <X size={14} />
      </button>
    </div>
  )
}

// ─── Main Page ────────────────────────────────────────────────────────────────

const SORT_OPTIONS = [
  { value: 'name-asc', label: 'Ime (A → Z)' },
  { value: 'name-desc', label: 'Ime (Z → A)' },
  { value: 'agents-desc', label: 'Agenti (više → manje)' },
  { value: 'agents-asc', label: 'Agenti (manje → više)' },
  { value: 'tickets-desc', label: 'Tiketi (više → manje)' },
  { value: 'tickets-asc', label: 'Tiketi (manje → više)' },
]

export default function TeamsPage() {
  const [teams, setTeams] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  const [search, setSearch] = useState('')
  const [categoryFilter, setCategoryFilter] = useState('')
  const [sortBy, setSortBy] = useState('name-asc')

  // Reassignment state
  const [reassignTarget, setReassignTarget] = useState(null) // { agent, team }
  const [reassignLoading, setReassignLoading] = useState(false)
  const [reassignError, setReassignError] = useState(null)

  // Toast
  const [toast, setToast] = useState(null)

  const load = async () => {
    setLoading(true)
    setError(null)
    try {
      const data = await getTeamsOverview()
      setTeams(data)
    } catch {
      setError('Greška pri učitavanju timova. Pokušajte ponovo.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    let cancelled = false
    async function fetchTeams() {
      setLoading(true)
      setError(null)
      try {
        const data = await getTeamsOverview()
        if (!cancelled) setTeams(data)
      } catch {
        if (!cancelled) setError('Greška pri učitavanju timova. Pokušajte ponovo.')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    fetchTeams()
    return () => { cancelled = true }
  }, [])

  // Unique categories for filter dropdown
  const categories = useMemo(() => {
    const seen = new Set()
    return teams.map(t => t.specializedCategory).filter(c => c && !seen.has(c) && seen.add(c))
  }, [teams])

  // Filtered + sorted teams
  const displayedTeams = useMemo(() => {
    let result = [...teams]

    // Search: by team name or member name
    if (search.trim()) {
      const q = search.toLowerCase()
      result = result.filter(t =>
        t.teamName.toLowerCase().includes(q) ||
        t.members.some(m =>
          `${m.firstName} ${m.lastName}`.toLowerCase().includes(q)
        )
      )
    }

    // Category filter
    if (categoryFilter) {
      result = result.filter(t => t.specializedCategory === categoryFilter)
    }

    // Sort
    result.sort((a, b) => {
      switch (sortBy) {
        case 'name-asc': return a.teamName.localeCompare(b.teamName)
        case 'name-desc': return b.teamName.localeCompare(a.teamName)
        case 'agents-desc': return b.activeAgentCount - a.activeAgentCount
        case 'agents-asc': return a.activeAgentCount - b.activeAgentCount
        case 'tickets-desc': return b.openTicketCount - a.openTicketCount
        case 'tickets-asc': return a.openTicketCount - b.openTicketCount
        default: return 0
      }
    })

    return result
  }, [teams, search, categoryFilter, sortBy])

  // Totals for summary bar
  const totalAgents = teams.reduce((s, t) => s + t.activeAgentCount, 0)
  const totalTickets = teams.reduce((s, t) => s + t.openTicketCount, 0)

  // ── Reassignment handlers ──────────────────────────────────────────────────
  const handleOpenReassign = (agent, team) => {
    setReassignTarget({ agent, team })
    setReassignError(null)
  }

  const handleCloseReassign = () => {
    if (reassignLoading) return
    setReassignTarget(null)
    setReassignError(null)
  }

  const handleConfirmReassign = async (agentId, newTeamId) => {
    if (!newTeamId) return
    setReassignLoading(true)
    setReassignError(null)
    try {
      await reassignAgent(agentId, newTeamId)
      setReassignTarget(null)
      setToast({ message: 'Agent je uspješno premješten u novi tim.', type: 'success' })
      await load() // Refresh stats
    } catch (err) {
      const msg = err?.response?.data?.message || 'Greška pri premještanju agenta.'
      setReassignError(msg)
    } finally {
      setReassignLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      {/* Page header */}
      <div className="flex items-center justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold text-navy-900">Timovi</h1>
          <p className="text-sm text-slate-500 mt-1">Pregled sastava timova i upravljanje agentima</p>
        </div>
        <button
          onClick={load}
          disabled={loading}
          className="flex items-center gap-2 px-3 py-2 rounded-lg border border-slate-200 text-sm text-slate-600 hover:bg-slate-50 transition-colors disabled:opacity-50"
        >
          <RefreshCw size={14} className={loading ? 'animate-spin' : ''} />
          Osvježi
        </button>
      </div>

      {/* Summary bar */}
      {!loading && !error && (
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-3">
          {[
            { label: 'Timova', value: teams.length, color: 'text-navy-700' },
            { label: 'Aktivnih agenata', value: totalAgents, color: 'text-emerald-600' },
            { label: 'Otvorenih tiketa', value: totalTickets, color: totalTickets > 0 ? 'text-amber-600' : 'text-emerald-600' },
            { label: 'Prikazano timova', value: displayedTeams.length, color: 'text-slate-600' },
          ].map(stat => (
            <div key={stat.label} className="bg-white rounded-xl border border-slate-200 px-4 py-3">
              <p className={`text-2xl font-bold ${stat.color}`}>{stat.value}</p>
              <p className="text-xs text-gray-400 mt-0.5">{stat.label}</p>
            </div>
          ))}
        </div>
      )}

      {/* Filters bar */}
      <div className="flex flex-wrap gap-3 items-center">
        {/* Search */}
        <div className="relative flex-1 min-w-[200px]">
          <Search size={15} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <input
            id="teams-search"
            type="text"
            placeholder="Pretraži po timu ili agentu…"
            value={search}
            onChange={e => setSearch(e.target.value)}
            className="w-full pl-9 pr-4 py-2.5 rounded-xl border border-slate-200 text-sm text-gray-800 placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-navy-200 focus:border-navy-300 bg-white"
          />
        </div>

        {/* Category filter */}
        <div className="relative">
          <Filter size={13} className="absolute left-3 top-1/2 -translate-y-1/2 text-gray-400" />
          <select
            id="teams-category-filter"
            value={categoryFilter}
            onChange={e => setCategoryFilter(e.target.value)}
            className="pl-8 pr-8 py-2.5 rounded-xl border border-slate-200 text-sm text-gray-700 focus:outline-none focus:ring-2 focus:ring-navy-200 focus:border-navy-300 bg-white appearance-none"
          >
            <option value="">Sve kategorije</option>
            {categories.map(c => (
              <option key={c} value={c}>{getCategoryMeta(c).label}</option>
            ))}
          </select>
        </div>

        {/* Sort */}
        <select
          id="teams-sort"
          value={sortBy}
          onChange={e => setSortBy(e.target.value)}
          className="px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-gray-700 focus:outline-none focus:ring-2 focus:ring-navy-200 focus:border-navy-300 bg-white"
        >
          {SORT_OPTIONS.map(o => (
            <option key={o.value} value={o.value}>{o.label}</option>
          ))}
        </select>

        {/* Clear filters */}
        {(search || categoryFilter || sortBy !== 'name-asc') && (
          <button
            onClick={() => { setSearch(''); setCategoryFilter(''); setSortBy('name-asc') }}
            className="flex items-center gap-1.5 px-3 py-2.5 rounded-xl border border-slate-200 text-sm text-slate-500 hover:bg-slate-50 transition-colors"
          >
            <X size={13} />
            Poništi filtere
          </button>
        )}
      </div>

      {/* Error */}
      {error && (
        <div className="rounded-xl bg-red-50 border border-red-200 p-4 flex items-center justify-between gap-3">
          <p className="text-sm text-red-800">{error}</p>
          <button
            onClick={load}
            className="rounded-lg border border-red-300 px-3 py-1.5 text-sm font-medium text-red-800 hover:bg-red-100"
          >
            Pokušaj ponovo
          </button>
        </div>
      )}

      {/* Loading */}
      {loading && (
        <div className="flex justify-center py-16">
          <div className="w-8 h-8 border-4 border-navy-200 border-t-navy-700 rounded-full animate-spin" />
        </div>
      )}

      {/* Empty state */}
      {!loading && !error && displayedTeams.length === 0 && (
        <div className="flex flex-col items-center py-16 text-center">
          <div className="w-14 h-14 rounded-2xl bg-slate-100 flex items-center justify-center mb-4">
            <Users size={24} className="text-slate-400" />
          </div>
          <p className="text-sm font-semibold text-gray-600">Nema timova</p>
          <p className="text-xs text-gray-400 mt-1">Nijedan tim ne odgovara zadanim kriterijima pretrage.</p>
        </div>
      )}

      {/* Teams grid */}
      {!loading && !error && displayedTeams.length > 0 && (
        <div className="grid gap-4 lg:grid-cols-2">
          {displayedTeams.map(team => (
            <TeamCard
              key={team.teamId}
              team={team}
              onReassign={handleOpenReassign}
            />
          ))}
        </div>
      )}

      {/* Reassign modal */}
      {reassignTarget && (
        <ReassignModal
          agent={reassignTarget.agent}
          currentTeam={reassignTarget.team}
          allTeams={teams}
          onConfirm={handleConfirmReassign}
          onClose={handleCloseReassign}
          loading={reassignLoading}
          error={reassignError}
        />
      )}

      {/* Toast */}
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onClose={() => setToast(null)}
        />
      )}

      {/* Slide-up animation */}
      <style>{`
        @keyframes slideUp {
          from { transform: translateY(16px); opacity: 0; }
          to   { transform: translateY(0);   opacity: 1; }
        }
        .animate-slideUp { animation: slideUp 0.25s ease-out; }
      `}</style>
    </div>
  )
}
