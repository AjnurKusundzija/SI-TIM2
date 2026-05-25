import { useEffect, useMemo, useState } from 'react'
import { Pencil, Trash2, Plus, Power, PowerOff, Package as PackageIcon } from 'lucide-react'
import Modal from '../components/common/Modal'
import ConfirmDialog from '../components/common/ConfirmDialog'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'
import {
  getCatalog,
  createCatalogPackage,
  updateCatalogPackage,
  deleteCatalogPackage,
  updateCatalogPackageStatus,
} from '../services/packageCatalogService'

const TYPE_OPTIONS = [
  { value: 'INTERNET', label: 'Internet' },
  { value: 'TV', label: 'TV' },
  { value: 'MOBILE', label: 'Mobilni' },
  { value: 'BUNDLE', label: 'Kombinovani' },
]

const TYPE_LABEL = TYPE_OPTIONS.reduce((acc, o) => ({ ...acc, [o.value]: o.label }), {})

const EMPTY_FORM = {
  name: '',
  type: 'INTERNET',
  description: '',
  price: '',
  status: 'ACTIVE',
}

function formatPrice(price) {
  const num = Number(price)
  if (Number.isNaN(num)) return ''
  return `${num.toFixed(2).replace('.', ',')} KM`
}

export default function PackageManagement() {
  const [packages, setPackages] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  const [formOpen, setFormOpen] = useState(false)
  const [editing, setEditing] = useState(null)
  const [form, setForm] = useState(EMPTY_FORM)
  const [formError, setFormError] = useState(null)
  const [saving, setSaving] = useState(false)

  const [confirmDeactivate, setConfirmDeactivate] = useState(null)
  const [deleteBlocked, setDeleteBlocked] = useState(null)
  const [confirmDelete, setConfirmDelete] = useState(null)

  const sorted = useMemo(
    () => [...packages].sort((a, b) => a.name.localeCompare(b.name, 'bs')),
    [packages]
  )

  const [refreshKey, setRefreshKey] = useState(0)
  const refresh = () => setRefreshKey((k) => k + 1)

  useEffect(() => {
    let cancelled = false
    async function load() {
      try {
        const data = await getCatalog()
        if (!cancelled) {
          setPackages(data)
          setError(null)
        }
      } catch (err) {
        console.error(err)
        if (!cancelled) setError('Greška pri učitavanju kataloga paketa.')
      } finally {
        if (!cancelled) setLoading(false)
      }
    }
    load()
    return () => {
      cancelled = true
    }
  }, [refreshKey])

  function openCreate() {
    setEditing(null)
    setForm(EMPTY_FORM)
    setFormError(null)
    setFormOpen(true)
  }

  function openEdit(pkg) {
    setEditing(pkg)
    setForm({
      name: pkg.name,
      type: pkg.type,
      description: pkg.description || '',
      price: String(pkg.price ?? ''),
      status: pkg.status,
    })
    setFormError(null)
    setFormOpen(true)
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setFormError(null)

    const name = form.name.trim()
    if (!name) {
      setFormError('Naziv paketa je obavezan.')
      return
    }
    const priceNum = Number(form.price)
    if (!Number.isFinite(priceNum) || priceNum <= 0) {
      setFormError('Cijena mora biti pozitivan broj.')
      return
    }

    const payload = {
      name,
      type: form.type,
      description: form.description.trim(),
      price: priceNum,
      status: form.status,
    }

    setSaving(true)
    try {
      if (editing) {
        await updateCatalogPackage(editing.catalogPackageId, payload)
      } else {
        await createCatalogPackage(payload)
      }
      setFormOpen(false)
      refresh()
    } catch (err) {
      const msg = err?.response?.data?.message || 'Greška pri spremanju paketa.'
      setFormError(msg)
    } finally {
      setSaving(false)
    }
  }

  async function handleToggleStatus(pkg) {
    const nextStatus = pkg.status === 'ACTIVE' ? 'INACTIVE' : 'ACTIVE'
    try {
      await updateCatalogPackageStatus(pkg.catalogPackageId, nextStatus)
      refresh()
    } catch (err) {
      console.error(err)
      setError(err?.response?.data?.message || 'Greška pri promjeni statusa.')
    } finally {
      setConfirmDeactivate(null)
    }
  }

  function attemptDelete(pkg) {
    if (pkg.activeSubscriptionCount > 0) {
      setDeleteBlocked(pkg)
      return
    }
    setConfirmDelete(pkg)
  }

  async function handleDelete(pkg) {
    try {
      await deleteCatalogPackage(pkg.catalogPackageId)
      refresh()
    } catch (err) {
      const msg = err?.response?.data?.message
      if (err?.response?.status === 409 && msg) {
        // Slučaj da je između nas i servera neko dodao pretplatu — re-fetch i pokaži dijalog.
        refresh()
        setDeleteBlocked({ ...pkg, activeSubscriptionCount: pkg.activeSubscriptionCount || 1, _serverMessage: msg })
      } else {
        setError(msg || 'Greška pri brisanju paketa.')
      }
    } finally {
      setConfirmDelete(null)
    }
  }

  return (
    <div className="space-y-5">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-xl font-semibold text-gray-900">Upravljanje paketima</h1>
          <p className="text-sm text-gray-500 mt-1">
            Definišite i uređujte katalog paketa koje firma nudi klijentima.
          </p>
        </div>
        <button
          onClick={openCreate}
          className="inline-flex items-center gap-2 px-4 py-2 rounded-lg bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium transition-colors"
        >
          <Plus size={16} />
          Novi paket
        </button>
      </div>

      {error && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
          {error}
        </div>
      )}

      {loading ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-10 text-center text-sm text-gray-500">
          Učitavanje...
        </div>
      ) : sorted.length === 0 ? (
        <EmptyState
          icon={PackageIcon}
          title="Katalog je prazan"
          description='Dodajte prvi paket pritiskom na "Novi paket".'
        />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead className="bg-gray-50 text-xs uppercase tracking-wider text-gray-500">
                <tr>
                  <th className="px-4 py-3 text-left">Naziv</th>
                  <th className="px-4 py-3 text-left">Tip</th>
                  <th className="px-4 py-3 text-left">Cijena</th>
                  <th className="px-4 py-3 text-left">Status</th>
                  <th className="px-4 py-3 text-left">Aktivne pretplate</th>
                  <th className="px-4 py-3 text-right">Akcije</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {sorted.map((pkg) => (
                  <tr key={pkg.catalogPackageId} className="hover:bg-gray-50">
                    <td className="px-4 py-3">
                      <div className="font-medium text-gray-900">{pkg.name}</div>
                      {pkg.description && (
                        <div className="text-xs text-gray-500 mt-0.5 line-clamp-1">{pkg.description}</div>
                      )}
                    </td>
                    <td className="px-4 py-3">
                      <Badge value={pkg.type} />
                    </td>
                    <td className="px-4 py-3 text-gray-900 whitespace-nowrap">{formatPrice(pkg.price)}</td>
                    <td className="px-4 py-3">
                      <Badge value={pkg.status} />
                    </td>
                    <td className="px-4 py-3 text-gray-700">{pkg.activeSubscriptionCount}</td>
                    <td className="px-4 py-3">
                      <div className="flex items-center justify-end gap-1">
                        <button
                          onClick={() => openEdit(pkg)}
                          className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-navy-700 hover:bg-navy-50 rounded-md transition-colors"
                          title="Uredi"
                        >
                          <Pencil size={14} />
                          Uredi
                        </button>
                        {pkg.status === 'ACTIVE' ? (
                          <button
                            onClick={() => setConfirmDeactivate(pkg)}
                            className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-amber-700 hover:bg-amber-50 rounded-md transition-colors"
                            title="Deaktiviraj"
                          >
                            <PowerOff size={14} />
                            Deaktiviraj
                          </button>
                        ) : (
                          <button
                            onClick={() => handleToggleStatus(pkg)}
                            className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-emerald-700 hover:bg-emerald-50 rounded-md transition-colors"
                            title="Aktiviraj"
                          >
                            <Power size={14} />
                            Aktiviraj
                          </button>
                        )}
                        <button
                          onClick={() => attemptDelete(pkg)}
                          className="inline-flex items-center gap-1 px-2.5 py-1.5 text-xs font-medium text-red-700 hover:bg-red-50 rounded-md transition-colors"
                          title="Obriši"
                        >
                          <Trash2 size={14} />
                          Obriši
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      <Modal
        isOpen={formOpen}
        onClose={() => setFormOpen(false)}
        title={editing ? 'Uredi paket' : 'Novi paket'}
      >
        <form onSubmit={handleSubmit} className="space-y-4">
          {formError && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
              {formError}
            </div>
          )}

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Naziv *</label>
            <input
              type="text"
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 focus:border-transparent"
              maxLength={100}
              required
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Tip *</label>
              <select
                value={form.type}
                onChange={(e) => setForm({ ...form, type: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 focus:border-transparent bg-white"
              >
                {TYPE_OPTIONS.map((o) => (
                  <option key={o.value} value={o.value}>
                    {o.label}
                  </option>
                ))}
              </select>
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Cijena (KM) *</label>
              <input
                type="number"
                step="0.01"
                min="0.01"
                value={form.price}
                onChange={(e) => setForm({ ...form, price: e.target.value })}
                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 focus:border-transparent"
                required
              />
            </div>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Opis</label>
            <textarea
              value={form.description}
              onChange={(e) => setForm({ ...form, description: e.target.value })}
              rows={3}
              maxLength={1000}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 focus:border-transparent resize-none"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Status</label>
            <select
              value={form.status}
              onChange={(e) => setForm({ ...form, status: e.target.value })}
              className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-navy-500 focus:border-transparent bg-white"
            >
              <option value="ACTIVE">Aktivan</option>
              <option value="INACTIVE">Neaktivan</option>
            </select>
          </div>

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => setFormOpen(false)}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Odustani
            </button>
            <button
              type="submit"
              disabled={saving}
              className="px-4 py-2 text-sm font-medium text-white bg-navy-700 rounded-lg hover:bg-navy-800 transition-colors disabled:opacity-50"
            >
              {saving ? 'Spremanje...' : editing ? 'Sačuvaj' : 'Kreiraj'}
            </button>
          </div>
        </form>
      </Modal>

      <ConfirmDialog
        isOpen={!!confirmDeactivate}
        onClose={() => setConfirmDeactivate(null)}
        onConfirm={() => confirmDeactivate && handleToggleStatus(confirmDeactivate)}
        title="Deaktivacija paketa"
        message={
          confirmDeactivate
            ? `Da li ste sigurni da želite deaktivirati paket "${confirmDeactivate.name}"? Postojeće pretplate ostaju aktivne, ali novi klijenti ga neće vidjeti.`
            : ''
        }
        confirmText="Deaktiviraj"
        cancelText="Odustani"
        variant="danger"
      />

      <ConfirmDialog
        isOpen={!!confirmDelete}
        onClose={() => setConfirmDelete(null)}
        onConfirm={() => confirmDelete && handleDelete(confirmDelete)}
        title="Brisanje paketa"
        message={
          confirmDelete
            ? `Da li ste sigurni da želite obrisati paket "${confirmDelete.name}"? Ova radnja se ne može poništiti.`
            : ''
        }
        confirmText="Obriši"
        cancelText="Odustani"
        variant="danger"
      />

      <Modal
        isOpen={!!deleteBlocked}
        onClose={() => setDeleteBlocked(null)}
        title="Brisanje nije moguće"
        size="sm"
      >
        <div className="space-y-4">
          <div className="flex items-start gap-3">
            <div className="flex-shrink-0 w-10 h-10 rounded-full bg-amber-100 flex items-center justify-center">
              <PackageIcon size={20} className="text-amber-700" />
            </div>
            <p className="text-sm text-gray-700">
              {deleteBlocked?._serverMessage ||
                `Paket ima ${deleteBlocked?.activeSubscriptionCount} aktivnih pretplata i ne može biti obrisan.`}
              <br />
              <span className="text-gray-500 mt-1 block">
                Umjesto brisanja, paket možete deaktivirati — postojeće pretplate ostaju aktivne, a novi klijenti ga neće vidjeti.
              </span>
            </p>
          </div>
          <div className="flex justify-end">
            <button
              onClick={() => setDeleteBlocked(null)}
              className="px-4 py-2 text-sm font-medium text-white bg-navy-700 rounded-lg hover:bg-navy-800 transition-colors"
            >
              U redu
            </button>
          </div>
        </div>
      </Modal>
    </div>
  )
}

export { TYPE_LABEL }
