import { useEffect, useMemo, useState } from 'react'
import PropTypes from 'prop-types'
import { CreditCard, PlusCircle, PowerOff } from 'lucide-react'
import Badge from '../common/Badge'
import Modal from '../common/Modal'
import ConfirmDialog from '../common/ConfirmDialog'
import {
  getClientSubscriptions,
  getActiveCatalog,
  assignSubscription,
  deactivateSubscription,
} from '../../services/packageCatalogService'

function formatPrice(price) {
  const num = Number(price)
  if (Number.isNaN(num)) return ''
  return `${num.toFixed(2).replace('.', ',')} KM`
}

function formatDate(value) {
  if (!value) return '—'
  const d = new Date(value)
  if (Number.isNaN(d.getTime())) return '—'
  return d.toLocaleDateString('hr-HR')
}

// PB-52 / US-77: Sekcija "Pretplate" za admina. Komponenta se NE renderira za
// agente i tehničare (gating se radi u UserProfile-u).
export default function ClientSubscriptionsSection({ clientId }) {
  const [subscriptions, setSubscriptions] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const [assignOpen, setAssignOpen] = useState(false)
  const [catalog, setCatalog] = useState([])
  const [catalogLoading, setCatalogLoading] = useState(false)
  const [selectedPackageId, setSelectedPackageId] = useState('')
  const [startDate, setStartDate] = useState(() => new Date().toISOString().slice(0, 10))
  const [assignError, setAssignError] = useState(null)
  const [assigning, setAssigning] = useState(false)

  const [confirmDeactivate, setConfirmDeactivate] = useState(null)
  const [refreshKey, setRefreshKey] = useState(0)
  const refresh = () => setRefreshKey((k) => k + 1)

  useEffect(() => {
    if (!clientId) return undefined
    let cancelled = false
    async function load() {
      try {
        const data = await getClientSubscriptions(clientId)
        if (!cancelled) {
          setSubscriptions(data)
          setError(null)
        }
      } catch (err) {
        console.error(err)
        if (!cancelled) setError(err?.response?.data?.message || 'Greška pri učitavanju pretplata.')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => {
      cancelled = true
    }
  }, [clientId, refreshKey])

  const activePackageIds = useMemo(
    () => new Set(subscriptions.filter((s) => s.status === 'ACTIVE').map((s) => s.catalogPackageId)),
    [subscriptions]
  )

  const availableCatalog = useMemo(
    () => catalog.filter((p) => !activePackageIds.has(p.catalogPackageId)),
    [catalog, activePackageIds]
  )

  async function openAssignModal() {
    setAssignOpen(true)
    setAssignError(null)
    setSelectedPackageId('')
    setStartDate(new Date().toISOString().slice(0, 10))
    setCatalogLoading(true)
    try {
      const data = await getActiveCatalog()
      setCatalog(data)
    } catch (err) {
      console.error(err)
      setAssignError('Greška pri učitavanju kataloga paketa.')
    } finally {
      setCatalogLoading(false)
    }
  }

  async function handleAssign(e) {
    e.preventDefault()
    setAssignError(null)
    if (!selectedPackageId) {
      setAssignError('Odaberite paket.')
      return
    }
    if (!startDate) {
      setAssignError('Datum početka je obavezan.')
      return
    }
    const pkgId = Number(selectedPackageId)
    if (activePackageIds.has(pkgId)) {
      setAssignError('Klijent već ima aktivnu pretplatu na ovaj paket.')
      return
    }
    setAssigning(true)
    try {
      await assignSubscription(clientId, pkgId, new Date(startDate).toISOString())
      setAssignOpen(false)
      refresh()
    } catch (err) {
      const msg = err?.response?.data?.message || 'Greška pri dodjeli paketa.'
      setAssignError(msg)
    } finally {
      setAssigning(false)
    }
  }

  async function handleDeactivate(sub) {
    try {
      await deactivateSubscription(clientId, sub.subscriptionId)
      refresh()
    } catch (err) {
      console.error(err)
      setError(err?.response?.data?.message || 'Greška pri ukidanju pretplate.')
    } finally {
      setConfirmDeactivate(null)
    }
  }

  const duplicateGuard =
    !!selectedPackageId && activePackageIds.has(Number(selectedPackageId))

  return (
    <section className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between mb-5">
        <div className="flex items-center gap-3 text-navy-700">
          <CreditCard size={18} />
          <h2 className="text-lg font-semibold">Pretplate</h2>
        </div>
        <button
          onClick={openAssignModal}
          className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium transition-colors"
        >
          <PlusCircle size={16} />
          Dodijeli paket
        </button>
      </div>

      {error && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
          {error}
        </div>
      )}

      {loading ? (
        <div className="text-sm text-slate-500">Učitavanje...</div>
      ) : subscriptions.length === 0 ? (
        <div className="rounded-3xl border border-dashed border-gray-200 p-8 text-sm text-slate-500">
          Klijent trenutno nema dodijeljenih pretplata.
        </div>
      ) : (
        <div className="space-y-3">
          {subscriptions.map((sub) => (
            <div
              key={sub.subscriptionId}
              className="rounded-2xl border border-gray-100 p-4 hover:shadow-sm transition"
            >
              <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                <div className="min-w-0">
                  <div className="flex items-center gap-2 flex-wrap">
                    <h3 className="text-base font-semibold text-gray-900">{sub.packageName}</h3>
                    <Badge value={sub.packageType} />
                    <Badge value={sub.status} />
                  </div>
                  {sub.packageDescription && (
                    <p className="text-sm text-slate-500 mt-1 line-clamp-2">{sub.packageDescription}</p>
                  )}
                  <div className="text-xs text-slate-500 mt-2 flex flex-wrap gap-x-4 gap-y-1">
                    <span>Datum početka: {formatDate(sub.startDate)}</span>
                    {sub.deactivatedDate && (
                      <span>Datum ukidanja: {formatDate(sub.deactivatedDate)}</span>
                    )}
                    <span>Cijena: {formatPrice(sub.price)}/mj</span>
                  </div>
                </div>

                {sub.status === 'ACTIVE' && (
                  <button
                    onClick={() => setConfirmDeactivate(sub)}
                    className="inline-flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-amber-700 bg-amber-50 hover:bg-amber-100 rounded-lg transition-colors self-start"
                  >
                    <PowerOff size={14} />
                    Ukini pretplatu
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      <Modal isOpen={assignOpen} onClose={() => setAssignOpen(false)} title="Dodijeli paket">
        <form onSubmit={handleAssign} className="space-y-4">
          {assignError && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
              {assignError}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Paket *</label>
            {catalogLoading ? (
              <div className="text-sm text-slate-500">Učitavanje paketa...</div>
            ) : (
              <select
                value={selectedPackageId}
                onChange={(e) => setSelectedPackageId(e.target.value)}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 bg-white"
                required
              >
                <option value="">— Odaberite paket —</option>
                {availableCatalog.map((p) => (
                  <option key={p.catalogPackageId} value={p.catalogPackageId}>
                    {p.name} — {formatPrice(p.price)}/mj
                  </option>
                ))}
              </select>
            )}
            {availableCatalog.length === 0 && !catalogLoading && (
              <p className="text-xs text-slate-500 mt-1">
                Nema dostupnih aktivnih paketa za dodjelu (klijent je već pretplaćen na sve).
              </p>
            )}
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Datum početka *</label>
            <input
              type="date"
              value={startDate}
              onChange={(e) => setStartDate(e.target.value)}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500"
              required
            />
          </div>

          {duplicateGuard && (
            <div className="p-3 bg-amber-50 border border-amber-200 rounded-lg text-sm text-amber-800">
              Klijent već ima aktivnu pretplatu na ovaj paket.
            </div>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setAssignOpen(false)}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Odustani
            </button>
            <button
              type="submit"
              disabled={assigning || duplicateGuard || !selectedPackageId}
              className="px-4 py-2 text-sm font-medium text-white bg-navy-700 rounded-lg hover:bg-navy-800 transition-colors disabled:opacity-50"
            >
              {assigning ? 'Dodjela...' : 'Dodijeli'}
            </button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!confirmDeactivate}
        onClose={() => setConfirmDeactivate(null)}
        onConfirm={() => confirmDeactivate && handleDeactivate(confirmDeactivate)}
        title="Ukidanje pretplate"
        message={
          confirmDeactivate
            ? `Da li ste sigurni da želite ukinuti pretplatu na paket "${confirmDeactivate.packageName}"? Status pretplate će se promijeniti u neaktivan.`
            : ''
        }
        confirmText="Ukini"
        cancelText="Odustani"
        variant="danger"
      />
    </section>
  )
}

ClientSubscriptionsSection.propTypes = {
  clientId: PropTypes.oneOfType([PropTypes.string, PropTypes.number]).isRequired,
}
