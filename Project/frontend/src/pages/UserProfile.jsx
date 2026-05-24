import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { ArrowLeft, Mail, Phone, MapPin, ClipboardList, Package, User, Edit, Save, X, ShieldAlert, CheckCircle } from 'lucide-react'
import { getUserProfile, updateUserDetails, deactivateUser, reactivateUser, getAgentTeams } from '../services/userService'
import { useAuth } from '../context/AuthContext'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'
import UserStatisticsPanel from '../components/common/UserStatisticsPanel'
import ClientSubscriptionsSection from '../components/admin/ClientSubscriptionsSection'

// ─── Validation rules per field ────────────────────────────────────────────────
function validateField(name, value, role) {
  switch (name) {
    case 'firstName':
      return value?.trim() ? '' : 'Ime je obavezno.'
    case 'lastName':
      return value?.trim() ? '' : 'Prezime je obavezno.'
    case 'phone':
      if (!value) return ''
      if (!/^0[6-9][0-9]{6,7}$/.test(value))
        return 'Unesite ispravan bosanski broj (npr. 061234567).'
      return ''
    case 'teamId':
      if (role === 'AGENT' && !value) return 'Odaberite tim za agenta.'
      return ''
    default:
      return ''
  }
}

// ─── Field wrapper with inline error ───────────────────────────────────────────
function Field({ label, error, children, hint }) {
  return (
    <div style={{ display: 'flex', flexDirection: 'column', gap: '6px' }}>
      <label
        style={{
          fontSize: '13px',
          fontWeight: 600,
          color: error ? '#dc2626' : '#374151',
          letterSpacing: '0.01em',
        }}
      >
        {label}
      </label>
      {children}
      {error ? (
        <span
          style={{
            fontSize: '12px',
            color: '#dc2626',
            display: 'flex',
            alignItems: 'center',
            gap: '4px',
          }}
        >
          <span style={{ fontSize: '14px', lineHeight: 1 }}>⚠</span> {error}
        </span>
      ) : hint ? (
        <span style={{ fontSize: '12px', color: '#9ca3af' }}>{hint}</span>
      ) : null}
    </div>
  )
}

const inputStyle = (hasError) => ({
  width: '100%',
  borderRadius: '12px',
  border: `1.5px solid ${hasError ? '#fca5a5' : '#d1d5db'}`,
  background: hasError ? '#fff5f5' : '#fff',
  padding: '11px 14px',
  fontSize: '14px',
  color: '#111827',
  outline: 'none',
  transition: 'border-color 0.15s, box-shadow 0.15s',
  boxSizing: 'border-box',
})

// ─── Toast ────────────────────────────────────────────────────────────────
function Toast({ title, message, type = 'success', onClose }) {
  useEffect(() => {
    const t = setTimeout(onClose, 6000)
    return () => clearTimeout(t)
  }, [onClose])

  const isError = type === 'error'
  const bgGradient = isError ? 'linear-gradient(135deg, #b91c1c 0%, #dc2626 100%)' : 'linear-gradient(135deg, #0f766e 0%, #0d9488 100%)'
  const shadow = isError ? 'rgba(220,38,38,0.35)' : 'rgba(13,148,136,0.35)'
  const Icon = isError ? ShieldAlert : CheckCircle

  return (
    <div
      style={{
        position: 'fixed',
        top: '24px',
        right: '24px',
        zIndex: 9999,
        animation: 'slideInRight 0.35s cubic-bezier(.22,1,.36,1)',
      }}
    >
      <style>{`
        @keyframes slideInRight {
          from { opacity: 0; transform: translateX(60px); }
          to   { opacity: 1; transform: translateX(0); }
        }
      `}</style>
      <div
        style={{
          display: 'flex',
          alignItems: 'flex-start',
          gap: '14px',
          background: bgGradient,
          color: '#fff',
          borderRadius: '16px',
          padding: '18px 22px',
          boxShadow: `0 8px 32px ${shadow}, 0 2px 8px rgba(0,0,0,0.15)`,
          minWidth: '320px',
          maxWidth: '420px',
        }}
      >
        <Icon size={26} style={{ flexShrink: 0, marginTop: '2px', opacity: 0.95 }} />
        <div style={{ flex: 1 }}>
          <p style={{ margin: 0, fontWeight: 700, fontSize: '15px', letterSpacing: '0.01em' }}>
            {title}
          </p>
          <p style={{ margin: '4px 0 0', fontSize: '13px', opacity: 0.88 }}>
            {message}
          </p>
        </div>
        <button
          onClick={onClose}
          style={{
            background: 'rgba(255,255,255,0.18)',
            border: 'none',
            borderRadius: '8px',
            padding: '4px',
            cursor: 'pointer',
            color: '#fff',
            display: 'flex',
            alignItems: 'center',
            flexShrink: 0,
          }}
        >
          <X size={16} />
        </button>
      </div>
    </div>
  )
}

export default function UserProfile() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { user: currentUser } = useAuth()
  const [profile, setProfile] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  // PB-52 / US-77 AC: kontrole pretplata postoje u DOM-u SAMO za admina.
  const canManageSubscriptions =
    currentUser?.role === 'ADMINISTRATOR' && profile?.role === 'CLIENT'

  const [isEditing, setIsEditing] = useState(false)
  const [editForm, setEditForm] = useState({})
  const [fieldErrors, setFieldErrors] = useState({})
  const [touched, setTouched] = useState({})
  const [toast, setToast] = useState(null)
  const [saving, setSaving] = useState(false)
  const [teams, setTeams] = useState([])
  const [showStatusConfirm, setShowStatusConfirm] = useState(false)
  const [statusAction, setStatusAction] = useState(null) // 'activate' or 'deactivate'
  const [statusLoading, setStatusLoading] = useState(false)
  const canEdit = currentUser?.role === 'ADMINISTRATOR' ||
    (currentUser?.role === 'AGENT' && profile?.role !== 'ADMINISTRATOR' && profile?.role !== 'AGENT')
  const canDeactivate = currentUser?.role === 'ADMINISTRATOR' ||
    (currentUser?.role === 'AGENT' && profile?.role === 'CLIENT')


  useEffect(() => {
    async function loadProfile() {
      if (!id) return

      setLoading(true)
      setError(null)

      try {
        const data = await getUserProfile(id)
        setProfile(data)
        setEditForm({
          firstName: data.firstName,
          lastName: data.lastName,
          phone: data.phone || '',
          location: data.location || '',
          teamId: data.teamId || ''
        })
      } catch (err) {
        setError(err?.response?.data?.message || 'Ne mogu učitati profil korisnika.')
      } finally {
        setLoading(false)
      }
    }

    loadProfile()
  }, [id])

  useEffect(() => {
    async function fetchTeams() {
      if (profile?.role === 'AGENT' && canEdit) {
        try {
          const fetchedTeams = await getAgentTeams()
          setTeams(fetchedTeams)

          // Try to find the team ID if the user has an expertise category
          if (profile.expertiseCategory) {
            const currentTeam = fetchedTeams.find(t => t.specializedCategory === profile.expertiseCategory)
            if (currentTeam) {
              setEditForm(prev => ({ ...prev, teamId: currentTeam.teamId }))
            }
          }
        } catch (err) {
          console.error('Failed to load agent teams:', err)
        }
      }
    }
    if (profile) fetchTeams()
  }, [profile, canEdit])

  const handleEditChange = (e) => {
    const { name, value } = e.target
    setEditForm(prev => ({ ...prev, [name]: value }))
    if (touched[name]) {
      setFieldErrors(prev => ({
        ...prev,
        [name]: validateField(name, value, profile.role),
      }))
    }
  }

  const handleBlur = (e) => {
    const { name, value } = e.target
    setTouched(prev => ({ ...prev, [name]: true }))
    setFieldErrors(prev => ({
      ...prev,
      [name]: validateField(name, value, profile.role),
    }))
  }

  const handleSave = async () => {
    const fields = ['firstName', 'lastName', 'phone', 'teamId']
    const errors = {}
    let hasError = false
    for (const f of fields) {
      const msg = validateField(f, editForm[f], profile.role)
      errors[f] = msg
      if (msg) hasError = true
    }
    setFieldErrors(errors)
    const allTouched = fields.reduce((a, k) => ({ ...a, [k]: true }), {})
    setTouched(allTouched)

    if (hasError) return

    setSaving(true)
    try {
      const payload = {
        firstName: editForm.firstName,
        lastName: editForm.lastName,
        phone: editForm.phone,
        location: editForm.location || null,
        teamId: profile.role === 'AGENT' && editForm.teamId ? parseInt(editForm.teamId, 10) : null
      }

      await updateUserDetails(profile.userId, payload)

      setProfile(prev => ({
        ...prev,
        firstName: payload.firstName,
        lastName: payload.lastName,
        phone: payload.phone,
        location: payload.location,
        teamId: payload.teamId,
        // Update expertise category locally for display
        expertiseCategory: payload.teamId ? teams.find(t => t.teamId === payload.teamId)?.specializedCategory : prev.expertiseCategory
      }))

      setIsEditing(false)
      setToast({ title: 'Uspješno', message: 'Podaci korisnika su ažurirani.', type: 'success' })
    } catch (err) {
      setToast({ title: 'Greška', message: err?.response?.data?.message || 'Ne mogu ažurirati podatke.', type: 'error' })
    } finally {
      setSaving(false)
    }
  }

  const handleStatusChange = async () => {
    setStatusLoading(true)
    try {
      if (statusAction === 'activate') {
        await reactivateUser(profile.userId)
        setProfile(prev => ({ ...prev, accountStatus: 'ACTIVE' }))
        setToast({ title: 'Uspješno', message: 'Korisnik je reaktiviran.', type: 'success' })
      } else {
        await deactivateUser(profile.userId)
        setProfile(prev => ({ ...prev, accountStatus: 'INACTIVE' }))
        setToast({ title: 'Uspješno', message: 'Korisnik je deaktiviran.', type: 'success' })
      }
    } catch (err) {
      const errorMsg = err?.response?.data?.message || 'Akcija nije uspjela.'
      setToast({ title: 'Greška', message: errorMsg, type: 'error' })
    } finally {
      setStatusLoading(false)
      setShowStatusConfirm(false)
    }
  }

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

  const isInactive = profile.accountStatus === 'INACTIVE'

  return (
    <div className="max-w-6xl mx-auto space-y-8 px-6 py-8 lg:px-10">
      {toast && <Toast title={toast.title} message={toast.message} type={toast.type} onClose={() => setToast(null)} />}
      <Link
        to={-1}
        className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-navy-700 transition-colors"
      >
        <ArrowLeft size={16} />
        Nazad
      </Link>

      <header className="rounded-3xl bg-white p-8 shadow-sm border border-slate-200">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div>
            <div className="flex items-center gap-3 mb-2">
              <p className="text-sm uppercase tracking-[0.24em] text-navy-600 font-semibold">Profil korisnika</p>
              {isInactive && (
                <span className="px-2 py-1 text-xs font-bold bg-red-100 text-red-700 rounded-md">DEAKTIVIRAN</span>
              )}
            </div>
            <h1 className="text-3xl font-semibold text-navy-900">{profile.firstName} {profile.lastName}</h1>
            <p className="text-sm text-slate-500 mt-1">Uloga: <span className="font-medium text-navy-700">{profile.role}</span></p>
          </div>

          <div className="flex flex-wrap items-center gap-3">
            {canDeactivate && currentUser.userId !== profile.userId && (
              <>
                {isInactive ? (
                  <button
                    onClick={() => { setStatusAction('activate'); setShowStatusConfirm(true); }}
                    disabled={statusLoading}
                    className="inline-flex items-center gap-2 rounded-2xl bg-green-50 px-4 py-2 text-sm font-semibold text-green-700 transition hover:bg-green-100 border border-green-200 disabled:opacity-50"
                  >
                    <CheckCircle size={16} />
                    Reaktiviraj nalog
                  </button>
                ) : (
                  <button
                    onClick={() => { setStatusAction('deactivate'); setShowStatusConfirm(true); }}
                    disabled={statusLoading}
                    className="inline-flex items-center gap-2 rounded-2xl bg-red-50 px-4 py-2 text-sm font-semibold text-red-600 transition hover:bg-red-100 border border-red-200 disabled:opacity-50"
                  >
                    <ShieldAlert size={16} />
                    Deaktiviraj nalog
                  </button>
                )}
              </>
            )}

            {canEdit && !isEditing && !isInactive && (
              <button
                onClick={() => setIsEditing(true)}
                className="inline-flex items-center gap-2 rounded-2xl bg-navy-50 px-4 py-2 text-sm font-semibold text-navy-700 transition hover:bg-navy-100 border border-navy-200"
              >
                <Edit size={16} />
                Uredi podatke
              </button>
            )}
          </div>
        </div>
      </header>

      {/* Confirmation Dialog for Status Change */}
      {showStatusConfirm && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
          <div className="w-full max-w-md rounded-3xl bg-white p-6 shadow-xl">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">
              {statusAction === 'activate' ? 'Potvrda reaktivacije' : 'Potvrda deaktivacije'}
            </h3>
            <p className="text-sm text-slate-500 mb-6">
              {statusAction === 'activate' ? (
                'Da li ste sigurni da želite reaktivirati ovog korisnika?'
              ) : (
                <>
                  Da li ste sigurni da želite deaktivirati ovog korisnika?
                  {(profile.role === 'AGENT' || profile.role === 'TECHNICIAN') ?
                    ' Ukoliko korisnik ima aktivne tikete, morat ćete ih prvo preusmjeriti.' : ''}
                </>
              )}
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setShowStatusConfirm(false)}
                className="px-4 py-2 text-sm font-medium text-slate-600 hover:bg-slate-50 rounded-xl"
              >
                Odustani
              </button>
              <button
                onClick={handleStatusChange}
                disabled={statusLoading}
                className={`px-4 py-2 text-sm font-medium text-white rounded-xl disabled:opacity-50 ${statusAction === 'activate' ? 'bg-green-600 hover:bg-green-700' : 'bg-red-600 hover:bg-red-700'
                  }`}
              >
                {statusLoading ? 'Obrada...' : (statusAction === 'activate' ? 'Reaktiviraj' : 'Deaktiviraj')}
              </button>
            </div>
          </div>
        </div>
      )}

    </div>
  )
}
