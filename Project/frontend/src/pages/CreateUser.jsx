import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { ArrowLeft, UserPlus, Save, CheckCircle, X } from 'lucide-react'
import { useAuth } from '../context/AuthContext'
import { createUser, getAgentTeams } from '../services/userService'

// ─── Validation rules per field ────────────────────────────────────────────────
const EMPTY_FORM = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: '',
  role: 'CLIENT',
  location: '',
  teamId: '',
}

const EMPTY_ERRORS = {
  firstName: '',
  lastName: '',
  email: '',
  phone: '',
  password: '',
  teamId: '',
}

function validateField(name, value, role) {
  switch (name) {
    case 'firstName':
      return value.trim() ? '' : 'Ime je obavezno.'
    case 'lastName':
      return value.trim() ? '' : 'Prezime je obavezno.'
    case 'email':
      if (!value.trim()) return 'Email je obavezan.'
      if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(value))
        return "Neispravan format email adrese (npr. ime@primjer.com)."
      return ''
    case 'phone':
      if (!value) return ''
      if (!/^0[6-9][0-9]{7,8}$/.test(value))
        return 'Unesite ispravan bosanski broj (npr. 061234567).'
      return ''
    case 'password':
      if (!value || value.length < 8) return 'Lozinka mora imati barem 8 karaktera.'
      if (!/[A-Z]/.test(value)) return 'Lozinka mora sadržavati najmanje jedno veliko slovo.'
      if (!/[!@#$%^&*(),.?":{}|<>]/.test(value))
        return 'Lozinka mora sadržavati najmanje jedan poseban znak (npr. !).'
      return ''
    case 'teamId':
      if (role === 'AGENT' && !value) return 'Odaberite tim za agenta.'
      return ''
    default:
      return ''
  }
}

function validateAll(formData) {
  const fields = ['firstName', 'lastName', 'email', 'phone', 'password', 'teamId']
  const errors = {}
  let hasError = false
  for (const f of fields) {
    const msg = validateField(f, formData[f], formData.role)
    errors[f] = msg
    if (msg) hasError = true
  }
  return { errors, hasError }
}

// ─── Success Toast ──────────────────────────────────────────────────────────────
function SuccessToast({ name, onClose }) {
  useEffect(() => {
    const t = setTimeout(onClose, 5000)
    return () => clearTimeout(t)
  }, [onClose])

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
          background: 'linear-gradient(135deg, #0f766e 0%, #0d9488 100%)',
          color: '#fff',
          borderRadius: '16px',
          padding: '18px 22px',
          boxShadow: '0 8px 32px rgba(13,148,136,0.35), 0 2px 8px rgba(0,0,0,0.15)',
          minWidth: '320px',
          maxWidth: '420px',
        }}
      >
        <CheckCircle size={26} style={{ flexShrink: 0, marginTop: '2px', opacity: 0.95 }} />
        <div style={{ flex: 1 }}>
          <p style={{ margin: 0, fontWeight: 700, fontSize: '15px', letterSpacing: '0.01em' }}>
            Korisnik kreiran!
          </p>
          <p style={{ margin: '4px 0 0', fontSize: '13px', opacity: 0.88 }}>
            <strong>{name}</strong> je uspješno dodan u sistem.
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

// ─── Main Component ─────────────────────────────────────────────────────────────
export default function CreateUser() {
  const navigate = useNavigate()
  const { user } = useAuth()

  const [formData, setFormData] = useState(EMPTY_FORM)
  const [fieldErrors, setFieldErrors] = useState(EMPTY_ERRORS)
  const [touched, setTouched] = useState({})
  const [teams, setTeams] = useState([])
  const [loading, setLoading] = useState(false)
  const [serverError, setServerError] = useState(null)
  const [toast, setToast] = useState(null) // { name: string }

  useEffect(() => {
    if (user?.role !== 'ADMINISTRATOR') navigate('/dashboard')
  }, [user, navigate])

  useEffect(() => {
    if (formData.role === 'AGENT') {
      getAgentTeams()
        .then(setTeams)
        .catch(() => { })
    }
  }, [formData.role])

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
    // Live-validate once the field has been touched
    if (touched[name]) {
      setFieldErrors(prev => ({
        ...prev,
        [name]: validateField(name, value, name === 'role' ? value : formData.role),
      }))
    }
  }

  const handleBlur = (e) => {
    const { name, value } = e.target
    setTouched(prev => ({ ...prev, [name]: true }))
    setFieldErrors(prev => ({
      ...prev,
      [name]: validateField(name, value, formData.role),
    }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setServerError(null)

    // Mark all fields touched and run full validation
    const allTouched = Object.keys(EMPTY_ERRORS).reduce((a, k) => ({ ...a, [k]: true }), {})
    setTouched(allTouched)

    const { errors, hasError } = validateAll(formData)
    setFieldErrors(errors)
    if (hasError) return

    setLoading(true)
    try {
      const payload = {
        ...formData,
        teamId: formData.role === 'AGENT' && formData.teamId ? parseInt(formData.teamId, 10) : null,
        location: formData.location || null,
      }
      await createUser(payload)

      const fullName = `${formData.firstName} ${formData.lastName}`
      setToast({ name: fullName })
      setFormData(EMPTY_FORM)
      setFieldErrors(EMPTY_ERRORS)
      setTouched({})
    } catch (err) {
      setServerError(
        err?.response?.data?.message || err?.message || 'Došlo je do greške prilikom kreiranja korisnika.'
      )
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ maxWidth: '860px', margin: '0 auto', padding: '32px 24px' }}>
      {/* Toast */}
      {toast && <SuccessToast name={toast.name} onClose={() => setToast(null)} />}

      {/* Back button */}
      <button
        onClick={() => navigate(-1)}
        style={{
          display: 'inline-flex',
          alignItems: 'center',
          gap: '6px',
          fontSize: '13px',
          color: '#6b7280',
          background: 'none',
          border: 'none',
          cursor: 'pointer',
          marginBottom: '24px',
          padding: '6px 0',
        }}
      >
        <ArrowLeft size={15} /> Nazad
      </button>

      {/* Header card */}
      <div
        style={{
          background: 'linear-gradient(135deg, #1e3a5f 0%, #1d4ed8 100%)',
          borderRadius: '20px',
          padding: '28px 32px',
          marginBottom: '24px',
          display: 'flex',
          alignItems: 'center',
          gap: '16px',
          boxShadow: '0 4px 24px rgba(29,78,216,0.2)',
        }}
      >
        <div
          style={{
            background: 'rgba(255,255,255,0.15)',
            borderRadius: '14px',
            padding: '12px',
            display: 'flex',
          }}
        >
          <UserPlus size={28} color="#fff" />
        </div>
        <div>
          <h1 style={{ margin: 0, fontSize: '22px', fontWeight: 700, color: '#fff' }}>
            Dodaj novog korisnika
          </h1>
          <p style={{ margin: '4px 0 0', fontSize: '13px', color: 'rgba(255,255,255,0.72)' }}>
            Popunite formu za kreiranje novog klijenta, agenta ili tehničara.
          </p>
        </div>
      </div>

      {/* Server error */}
      {serverError && (
        <div
          style={{
            background: '#fef2f2',
            border: '1.5px solid #fca5a5',
            borderRadius: '14px',
            padding: '14px 18px',
            marginBottom: '20px',
            fontSize: '14px',
            color: '#b91c1c',
            display: 'flex',
            gap: '10px',
            alignItems: 'flex-start',
          }}
        >
          <span style={{ fontSize: '18px', lineHeight: 1 }}>⚠</span>
          <span>{serverError}</span>
        </div>
      )}

      {/* Form card */}
      <form
        onSubmit={handleSubmit}
        noValidate
        style={{
          background: '#fff',
          borderRadius: '20px',
          padding: '32px',
          boxShadow: '0 1px 3px rgba(0,0,0,0.08), 0 4px 16px rgba(0,0,0,0.05)',
          border: '1px solid #e5e7eb',
          display: 'flex',
          flexDirection: 'column',
          gap: '28px',
        }}
      >
        {/* Row 1: Ime + Prezime */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <Field label="Ime *" error={fieldErrors.firstName}>
            <input
              id="firstName"
              type="text"
              name="firstName"
              value={formData.firstName}
              onChange={handleChange}
              onBlur={handleBlur}
              placeholder="Unesite ime"
              style={inputStyle(!!fieldErrors.firstName)}
            />
          </Field>
          <Field label="Prezime *" error={fieldErrors.lastName}>
            <input
              id="lastName"
              type="text"
              name="lastName"
              value={formData.lastName}
              onChange={handleChange}
              onBlur={handleBlur}
              placeholder="Unesite prezime"
              style={inputStyle(!!fieldErrors.lastName)}
            />
          </Field>
        </div>

        {/* Row 2: Email + Telefon */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <Field label="Email adresa *" error={fieldErrors.email}>
            <input
              id="email"
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              onBlur={handleBlur}
              placeholder="npr. ime@primjer.com"
              style={inputStyle(!!fieldErrors.email)}
            />
          </Field>
          <Field
            label="Broj telefona"
            error={fieldErrors.phone}
            hint="Format: 061234567"
          >
            <input
              id="phone"
              type="text"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              onBlur={handleBlur}
              placeholder="061 234 567"
              style={inputStyle(!!fieldErrors.phone)}
            />
          </Field>
        </div>

        {/* Row 3: Lozinka + Lokacija */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <Field
            label="Lozinka *"
            error={fieldErrors.password}
            hint="Min. 8 znakova, 1 veliko slovo, 1 poseban znak"
          >
            <input
              id="password"
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              onBlur={handleBlur}
              placeholder="Unesite lozinku"
              style={inputStyle(!!fieldErrors.password)}
            />
          </Field>
          <Field label="Lokacija">
            <select
              id="location"
              name="location"
              value={formData.location}
              onChange={handleChange}
              style={{ ...inputStyle(false), cursor: 'pointer' }}
            >
              <option value="">Odaberite lokaciju...</option>
              <option value="SARAJEVO">Sarajevo</option>
              <option value="TUZLA">Tuzla</option>
              <option value="ZENICA">Zenica</option>
              <option value="BIHAC">Bihać</option>
              <option value="MOSTAR">Mostar</option>
              <option value="BANJA_LUKA">Banja Luka</option>
              <option value="DOBOJ">Doboj</option>
              <option value="PRIJEDOR">Prijedor</option>
              <option value="TREBINJE">Trebinje</option>
              <option value="BIJELJINA">Bijeljina</option>
            </select>
          </Field>
        </div>

        {/* Row 4: Uloga + Tim (conditionally) */}
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
          <Field label="Uloga *">
            <select
              id="role"
              name="role"
              value={formData.role}
              onChange={handleChange}
              style={{ ...inputStyle(false), cursor: 'pointer' }}
            >
              <option value="CLIENT">Klijent</option>
              <option value="TECHNICIAN">Tehničar</option>
              <option value="AGENT">Agent</option>
            </select>
          </Field>
          {formData.role === 'AGENT' && (
            <Field label="Ekspertiza (Tim) *" error={fieldErrors.teamId}>
              <select
                id="teamId"
                name="teamId"
                value={formData.teamId}
                onChange={handleChange}
                onBlur={handleBlur}
                style={{ ...inputStyle(!!fieldErrors.teamId), cursor: 'pointer' }}
              >
                <option value="">Odaberite ekspertizu...</option>
                {teams.map(team => (
                  <option key={team.teamId} value={team.teamId}>
                    {team.teamName}{team.specializedCategory ? ` (${team.specializedCategory})` : ''}
                  </option>
                ))}
              </select>
            </Field>
          )}
        </div>

        {/* Submit */}
        <div style={{ display: 'flex', justifyContent: 'flex-end', paddingTop: '8px', borderTop: '1px solid #f3f4f6' }}>
          <button
            type="submit"
            disabled={loading}
            style={{
              display: 'inline-flex',
              alignItems: 'center',
              gap: '8px',
              background: loading ? '#93c5fd' : 'linear-gradient(135deg, #1e3a5f 0%, #1d4ed8 100%)',
              color: '#fff',
              border: 'none',
              borderRadius: '12px',
              padding: '12px 28px',
              fontSize: '14px',
              fontWeight: 700,
              cursor: loading ? 'not-allowed' : 'pointer',
              boxShadow: loading ? 'none' : '0 4px 14px rgba(29,78,216,0.35)',
              transition: 'all 0.2s',
              letterSpacing: '0.02em',
            }}
          >
            {loading ? (
              <span
                style={{
                  width: '18px',
                  height: '18px',
                  border: '2.5px solid rgba(255,255,255,0.4)',
                  borderTopColor: '#fff',
                  borderRadius: '50%',
                  display: 'inline-block',
                  animation: 'spin 0.75s linear infinite',
                }}
              />
            ) : (
              <Save size={17} />
            )}
            <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
            {loading ? 'Kreiranje...' : 'Sačuvaj korisnika'}
          </button>
        </div>
      </form>
    </div>
  )
}
