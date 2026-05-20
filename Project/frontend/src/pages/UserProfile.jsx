import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Mail, Phone, MapPin, ClipboardList, Package, User } from 'lucide-react'
import { getUserProfile } from '../services/userService'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'

export default function UserProfile() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    async function loadProfile() {
      if (!id) return

      setLoading(true)
      setError(null)

      try {
        const data = await getUserProfile(id)
        setProfile(data)
      } catch (err) {
        setError(err?.response?.data?.message || 'Ne mogu učitati profil korisnika.')
      } finally {
        setLoading(false)
      }
    }

    loadProfile()
  }, [id])

  if (loading) {
    return (
      <div className="max-w-5xl mx-auto py-10 space-y-4">
        <div className="h-8 w-40 bg-gray-200 rounded animate-pulse" />
        <div className="grid grid-cols-1 gap-4 lg:grid-cols-3">
          <div className="h-40 bg-gray-200 rounded-xl animate-pulse" />
          <div className="h-40 bg-gray-200 rounded-xl animate-pulse" />
          <div className="h-40 bg-gray-200 rounded-xl animate-pulse" />
        </div>
      </div>
    )
  }

  if (error || !profile) {
    return (
      <EmptyState
        title="Profil nije dostupan"
        description={error || 'Izabrani korisnik nije pronađen ili nemate pristup.'}
        action={() => navigate('/tickets')}
        actionLabel="Nazad na tikete"
      />
    )
  }

  return (
    <div className="max-w-6xl mx-auto space-y-8 px-6 py-8 lg:px-10">
      <Link
        to="/tickets"
        className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-navy-700 transition-colors"
      >
        <ArrowLeft size={16} />
        Nazad na tikete
      </Link>

      <header className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
        <div className="flex flex-col gap-3 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <p className="text-sm uppercase tracking-[0.24em] text-navy-600 font-semibold">Profil korisnika</p>
            <h1 className="text-3xl font-semibold text-navy-900">{profile.firstName} {profile.lastName}</h1>
            <p className="max-w-2xl text-sm text-slate-500 mt-1">
              Detaljni kontekst korisnika za podršku, uključujući informacije o profilu, pakete i historiju tiketa.
            </p>
          </div>
        </div>
      </header>

      <section className="grid gap-4 xl:grid-cols-3">
        <div className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200 space-y-5">
          <div className="flex items-center gap-3 text-navy-700">
            <User size={18} />
            <h2 className="text-lg font-semibold">Osnovni podaci</h2>
          </div>

          <div className="space-y-4 text-sm text-slate-700">
                        <div>
              <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Ime</p>
              <p>{profile.firstName}</p>
            </div>
                        <div>
              <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Prezime</p>
              <p>{profile.lastName}</p>
            </div>
            <div>
              <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Email</p>
              <p>{profile.email}</p>
            </div>
            {/*   PRIKAZATI NAKON STO UVEDEMO BROJEVE TELEFONA
            <div>
              <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Telefon</p>
              <p>{profile.phone || 'Nije unesen'}</p>
            </div>
            */}
            <div>
              <p className="text-xs uppercase tracking-[0.24em] text-slate-400">Lokacija</p>
              <p>{profile.location || 'Nije unesena'}</p>
            </div>
          </div>
        </div>

        <div className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200 xl:col-span-2">
          <div className="flex items-center gap-3 text-navy-700 mb-5">
            <Package size={18} />
            <h2 className="text-lg font-semibold">Aktivni paketi i pretplate</h2>
          </div>

          {profile.activePackages.length > 0 ? (
            <div className="space-y-4">
              {profile.activePackages.map((pkg) => (
                <div key={pkg.packageId} className="rounded-3xl border border-gray-100 p-4 hover:shadow-sm transition">
                  <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                    <div>
                      <p className="text-sm text-gray-500 uppercase tracking-[0.24em]">{pkg.packageType}</p>
                      <h3 className="text-lg font-semibold text-gray-900 mt-1">{pkg.packageName}</h3>
                      <p className="text-sm text-slate-500 mt-1">{pkg.packageDescription}</p>
                    </div>
                    <div className="flex items-center gap-3">
                      <Badge value={pkg.packageStatus} />
                      <span className="text-sm font-semibold text-gray-900">{pkg.monthlyPrice} KM/mj</span>
                    </div>
                  </div>
                  {pkg.summary && (
                    <p className="text-sm text-slate-500 mt-3">{pkg.summary}</p>
                  )}
                </div>
              ))}
            </div>
          ) : (
            <div className="rounded-3xl border border-dashed border-gray-200 p-8 text-sm text-slate-500">
              <p>Korisnik nema aktivne pakete ili pretplate.</p>
            </div>
          )}
        </div>
      </section>

      <section className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
        <div className="flex items-center gap-3 text-navy-700 mb-5">
          <ClipboardList size={18} />
          <h2 className="text-lg font-semibold">Historija tiketa korisnika</h2>
        </div>

        {profile.ticketHistory.length > 0 ? (
          <div className="space-y-3">
            {profile.ticketHistory.map((ticket) => (
              <button
                key={ticket.ticketId}
                type="button"
                onClick={() => navigate(`/tickets/${ticket.ticketId}`)}
                className="w-full text-left rounded-3xl border border-gray-100 p-4 hover:border-navy-200 hover:bg-slate-50 transition"
              >
                <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <p className="text-sm font-semibold text-gray-900">{ticket.title}</p>
                    <p className="text-xs text-slate-500 mt-1">{ticket.problemCategory} • Kreirano: {new Date(ticket.createdDate).toLocaleDateString('hr-HR')}</p>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Badge value={ticket.status} />
                    <Badge value={ticket.priority} />
                  </div>
                </div>
              </button>
            ))}
          </div>
        ) : (
          <div className="rounded-3xl border border-dashed border-gray-200 p-8 text-sm text-slate-500">
            <p>Ovaj korisnik još nema historiju tiketa.</p>
          </div>
        )}
      </section>
    </div>
  )
}
