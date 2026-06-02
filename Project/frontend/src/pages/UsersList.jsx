import { useState, useEffect } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'
import { Search, Plus, UserX, UserCheck, Edit, ShieldAlert } from 'lucide-react'
import { getUsersList } from '../services/userService'
import { useAuth } from '../context/AuthContext'
import Badge from '../components/common/Badge'

export default function UsersList() {
  const navigate = useNavigate()
  const location = useLocation()
  const { user } = useAuth()

  // Determine what type of users to show based on URL
  const isClients = location.pathname.includes('/clients')
  const isAgents = location.pathname.includes('/agents')
  const isTechnicians = location.pathname.includes('/technicians')
  const isDeactivated = location.pathname.includes('/deactivated')

  const roleFilter = isClients ? 'CLIENT' : isAgents ? 'AGENT' : isTechnicians ? 'TECHNICIAN' : ''
  const statusFilter = isDeactivated ? 'INACTIVE' : 'ACTIVE'

  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)
  
  const [page, setPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)
  const [search, setSearch] = useState('')
  const [locationFilter, setLocationFilter] = useState('')
  const [deactivatedRoleFilter, setDeactivatedRoleFilter] = useState('')
  const [availabilityFilter, setAvailabilityFilter] = useState('')

  useEffect(() => {
    // Agents cannot access /agents or /deactivated
    if (user?.role === 'AGENT' && (isAgents || isDeactivated)) {
      navigate('/dashboard')
    }
  }, [user, isAgents, isDeactivated, navigate])

  useEffect(() => {
    const t = setTimeout(() => {
      setPage(1)
      setSearch('')
      setLocationFilter('')
      setDeactivatedRoleFilter('')
    }, 0)
    return () => clearTimeout(t)
  }, [location.pathname])

  useEffect(() => {
    async function loadUsers() {
      setLoading(true)
      try {
        const activeRole = isDeactivated && deactivatedRoleFilter ? deactivatedRoleFilter : roleFilter
        
        const params = {
          page,
          pageSize: 10,
          role: activeRole,
          status: statusFilter,
          availability: availabilityFilter || undefined,
          search: search || undefined,
          location: locationFilter || undefined
        }
        
        const data = await getUsersList(params)
        setUsers(data.users)
        setTotalPages(data.totalPages || 1)
        setError(null)
      } catch (err) {
        setError(err?.response?.data?.message || 'Ne mogu učitati korisnike.')
      } finally {
        setLoading(false)
      }
    }

    const timer = setTimeout(() => {
      loadUsers()
    }, 300) // debounce search

    return () => clearTimeout(timer)
  }, [page, search, locationFilter, roleFilter, statusFilter, deactivatedRoleFilter, availabilityFilter, isDeactivated])

  let pageTitle = 'Korisnici'
  if (isClients) pageTitle = 'Klijenti'
  else if (isAgents) pageTitle = 'Agenti'
  else if (isTechnicians) pageTitle = 'Tehničari'
  else if (isDeactivated) pageTitle = 'Deaktivirani nalozi'

  return (
    <div className="space-y-6">
      <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-semibold text-navy-900">{pageTitle}</h1>
          <p className="text-sm text-slate-500 mt-1">
            Pregled i upravljanje korisničkim nalozima.
          </p>
        </div>
        {user?.role === 'ADMINISTRATOR' && !isDeactivated && (
          <button
            onClick={() => navigate('/users/add')}
            className="inline-flex items-center gap-2 rounded-2xl bg-navy-700 px-5 py-2.5 text-sm font-semibold text-white transition hover:bg-navy-800 shrink-0"
          >
            <Plus size={18} />
            Dodaj korisnika
          </button>
        )}
      </header>

      <div className="rounded-3xl bg-white p-6 shadow-sm border border-slate-200">
        <div className="flex flex-col gap-4 md:flex-row md:items-center md:justify-between mb-6">
          <div className="relative flex-1 max-w-md">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" size={18} />
            <input
              type="text"
              placeholder="Pretraži po imenu, emailu ili telefonu..."
              value={search}
              onChange={(e) => {
                setSearch(e.target.value)
                setPage(1)
              }}
              className="w-full rounded-2xl border border-slate-300 pl-10 pr-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200"
            />
          </div>
          
          <div className="flex flex-wrap gap-3">
              {user?.role === 'ADMINISTRATOR' && (
                <select
                  value={availabilityFilter}
                  onChange={(e) => {
                    setAvailabilityFilter(e.target.value)
                    setPage(1)
                  }}
                  className="rounded-2xl border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200 bg-white"
                >
                  <option value="">Sve dostupnosti</option>
                  <option value="AVAILABLE">Dostupan</option>
                  <option value="BUSY">Zauzet</option>
                  <option value="UNAVAILABLE">Nedostupan</option>
                </select>
              )}
            {isDeactivated && (
               <select
               value={deactivatedRoleFilter}
               onChange={(e) => {
                 setDeactivatedRoleFilter(e.target.value)
                 setPage(1)
               }}
               className="rounded-2xl border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200 bg-white"
             >
               <option value="">Sve uloge</option>
               <option value="CLIENT">Klijenti</option>
               <option value="AGENT">Agenti</option>
               <option value="TECHNICIAN">Tehničari</option>
             </select>
            )}

            <select
              value={locationFilter}
              onChange={(e) => {
                setLocationFilter(e.target.value)
                setPage(1)
              }}
              className="rounded-2xl border border-slate-300 px-4 py-2 text-sm focus:border-navy-500 focus:outline-none focus:ring-2 focus:ring-navy-200 bg-white"
            >
              <option value="">Sve lokacije</option>
              <option value="SARAJEVO">Sarajevo</option>
              <option value="TUZLA">Tuzla</option>
              <option value="ZENICA">Zenica</option>
              <option value="BIHAC">Bihać</option>
              <option value="MOSTAR">Mostar</option>
              <option value="BANJA_LUKA">Banja Luka</option>
            </select>
          </div>
        </div>

        {error ? (
          <div className="p-8 text-center text-red-600 bg-red-50 rounded-2xl border border-red-100">
            {error}
          </div>
        ) : loading ? (
          <div className="space-y-4">
            {[1, 2, 3, 4, 5].map(i => (
              <div key={i} className="h-16 bg-slate-100 animate-pulse rounded-2xl" />
            ))}
          </div>
        ) : users.length === 0 ? (
          <div className="py-12 text-center text-slate-500 flex flex-col items-center gap-3">
            <ShieldAlert size={40} className="text-slate-300" />
            <p>Nema pronađenih korisnika po zadatim kriterijima.</p>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left text-sm text-slate-600">
              <thead className="bg-slate-50 text-xs uppercase text-slate-500">
                <tr>
                  <th className="px-4 py-3 font-medium rounded-tl-2xl">Korisnik</th>
                  <th className="px-4 py-3 font-medium">Email</th>
                  <th className="px-4 py-3 font-medium">Telefon</th>
                  <th className="px-4 py-3 font-medium">Lokacija</th>
                  <th className="px-4 py-3 font-medium">Dostupnost</th>
                  {(!roleFilter || isDeactivated) && <th className="px-4 py-3 font-medium">Uloga</th>}
                  {isAgents && <th className="px-4 py-3 font-medium">Otvoreni tiketi</th>}
                  {isAgents && <th className="px-4 py-3 font-medium">Ekspertiza</th>}
                  <th className="px-4 py-3 font-medium text-right rounded-tr-2xl">Akcije</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {users.map(u => (
                  <tr key={u.userId} className="hover:bg-slate-50 transition-colors">
                    <td className="px-4 py-4">
                      <div className="font-semibold text-navy-900">{u.firstName} {u.lastName}</div>
                    </td>
                    <td className="px-4 py-4">{u.email}</td>
                    <td className="px-4 py-4">{u.phone || '-'}</td>
                    <td className="px-4 py-4">{u.location || '-'}</td>
                    <td className="px-4 py-4">{u.availability ? <Badge value={u.availability} /> : '-'}</td>
                    {(!roleFilter || isDeactivated) && (
                      <td className="px-4 py-4">
                        <Badge value={u.role} />
                      </td>
                    )}
                    {isAgents && (
                      <>
                      <td className="px-4 py-4 text-xs font-semibold uppercase text-slate-500">{u.expertiseCategory || '-'}</td>
                      <td className="px-4 py-4 text-sm font-medium text-navy-700">{u.openAssignedTicketsCount ?? 0}</td>
                      </>
                    )}
                    <td className="px-4 py-4 text-right">
                      <button
                        onClick={() => navigate(`/users/${u.userId}`)}
                        className="inline-flex items-center gap-1.5 rounded-xl bg-navy-50 px-3 py-1.5 text-xs font-medium text-navy-700 hover:bg-navy-100 transition-colors"
                      >
                        <Edit size={14} />
                        Detalji
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {totalPages > 1 && (
          <div className="flex items-center justify-center gap-2 mt-6">
            <button
              onClick={() => setPage(p => Math.max(1, p - 1))}
              disabled={page === 1}
              className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-xl hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Prethodna
            </button>
            <span className="text-sm text-slate-500">
              Stranica {page} od {totalPages}
            </span>
            <button
              onClick={() => setPage(p => Math.min(totalPages, p + 1))}
              disabled={page === totalPages}
              className="px-4 py-2 text-sm font-medium text-slate-700 bg-white border border-slate-300 rounded-xl hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              Sljedeća
            </button>
          </div>
        )}
      </div>
    </div>
  )
}
