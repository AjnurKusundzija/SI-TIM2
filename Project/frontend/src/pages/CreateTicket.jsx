// PB-22: Kreiraj novi tiket
import { useState } from 'react'
import { createTicket } from '../services/ticketService'

const PROBLEM_CATEGORIES = [
  { value: 'INTERNET', label: 'Internet' },
  { value: 'TV', label: 'TV' },
  { value: 'MOBILE_NETWORK', label: 'Mobilna mreža' },
  { value: 'BILLING', label: 'Naplate' },
  { value: 'TECHNICAL_SUPPORT', label: 'Tehnička podrška' },
]

const PRIORITIES = [
  { value: 'LOW', label: 'Nizak' },
  { value: 'MEDIUM', label: 'Srednji' },
  { value: 'HIGH', label: 'Visok' },
]

export default function CreateTicket() {
  const [formData, setFormData] = useState({
    subject: '',
    type: '',
    description: '',
    priority: '',
  })
  const [errors, setErrors] = useState({})
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState(null)

  const validateForm = () => {
    const newErrors = {}
    if (!formData.subject.trim()) {
      newErrors.subject = 'Predmet je obavezan.'
    }
    if (!formData.type) {
      newErrors.type = 'Vrsta problema je obavezna.'
    }
    if (!formData.description.trim()) {
      newErrors.description = 'Opis je obavezan.'
    }
    if (!formData.priority) {
      newErrors.priority = 'Prioritet je obavezan.'
    }
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({ ...prev, [name]: value }))
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }))
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return

    setLoading(true)
    setError(null)
    setSuccess(false)

    try {
      await createTicket({
        subject: formData.subject.trim(),
        type: formData.type,
        description: formData.description.trim(),
        priority: formData.priority,
      })
      setSuccess(true)
      setFormData({
        subject: '',
        type: '',
        description: '',
        priority: '',
      })
    } catch (err) {
      console.error(err)
      if (err.response?.status === 401) {
        setError('Niste autorizovani. Molimo prijavite se ponovo.')
      } else if (err.response?.status === 400) {
        setError('Neispravni podaci. Provjerite unos.')
      } else {
        setError('Greška pri kreiranju tiketa. Pokušajte ponovo.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={styles.page}>
      <div style={styles.container}>
        <div style={styles.header}>
          <h1 style={styles.title}>Kreiraj novi tiket</h1>
          <p style={styles.subtitle}>Prijavite vaš problem ili zahtjev</p>
        </div>

        <form onSubmit={handleSubmit} style={styles.form}>
          <div style={styles.field}>
            <label htmlFor="subject" style={styles.label}>Predmet *</label>
            <input
              type="text"
              id="subject"
              name="subject"
              value={formData.subject}
              onChange={handleChange}
              style={styles.input}
              placeholder="Kratak opis problema"
            />
            {errors.subject && <span style={styles.error}>{errors.subject}</span>}
          </div>

          <div style={styles.field}>
            <label htmlFor="type" style={styles.label}>Vrsta problema *</label>
            <select
              id="type"
              name="type"
              value={formData.type}
              onChange={handleChange}
              style={styles.select}
            >
              <option value="">Odaberite vrstu</option>
              {PROBLEM_CATEGORIES.map(cat => (
                <option key={cat.value} value={cat.value}>{cat.label}</option>
              ))}
            </select>
            {errors.type && <span style={styles.error}>{errors.type}</span>}
          </div>

          <div style={styles.field}>
            <label htmlFor="priority" style={styles.label}>Prioritet *</label>
            <select
              id="priority"
              name="priority"
              value={formData.priority}
              onChange={handleChange}
              style={styles.select}
            >
              <option value="">Odaberite prioritet</option>
              {PRIORITIES.map(pri => (
                <option key={pri.value} value={pri.value}>{pri.label}</option>
              ))}
            </select>
            {errors.priority && <span style={styles.error}>{errors.priority}</span>}
          </div>

          <div style={styles.field}>
            <label htmlFor="description" style={styles.label}>Opis *</label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              style={styles.textarea}
              placeholder="Detaljan opis problema ili zahtjeva"
              rows={6}
            />
            {errors.description && <span style={styles.error}>{errors.description}</span>}
          </div>

          {error && <p style={styles.globalError}>{error}</p>}
          {success && <p style={styles.success}>Tiket je uspješno kreiran!</p>}

          <button type="submit" disabled={loading} style={{
            ...styles.button,
            background: loading ? '#9ca3af' : '#3b82f6',
            cursor: loading ? 'not-allowed' : 'pointer',
          }}>
            {loading ? 'Kreiranje...' : 'Kreiraj tiket'}
          </button>
        </form>
      </div>
    </div>
  )
}

const styles = {
  page: {
    minHeight: '100vh',
    padding: '40px 20px',
    background: '#f9fafb',
    display: 'flex',
    justifyContent: 'center',
  },
  container: {
    width: '100%',
    maxWidth: '600px',
  },
  header: {
    marginBottom: '32px',
  },
  title: {
    fontSize: '30px',
    margin: '0 0 6px',
    fontWeight: 700,
    color: '#111827',
  },
  subtitle: {
    fontSize: '14px',
    color: '#6b7280',
  },
  form: {
    background: '#ffffff',
    padding: '32px',
    borderRadius: '8px',
    boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
  },
  field: {
    marginBottom: '20px',
  },
  label: {
    display: 'block',
    fontSize: '14px',
    fontWeight: 500,
    color: '#374151',
    marginBottom: '6px',
  },
  input: {
    width: '100%',
    padding: '10px 12px',
    border: '1px solid #d1d5db',
    borderRadius: '6px',
    fontSize: '14px',
    boxSizing: 'border-box',
  },
  select: {
    width: '100%',
    padding: '10px 12px',
    border: '1px solid #d1d5db',
    borderRadius: '6px',
    fontSize: '14px',
    boxSizing: 'border-box',
    background: '#ffffff',
  },
  textarea: {
    width: '100%',
    padding: '10px 12px',
    border: '1px solid #d1d5db',
    borderRadius: '6px',
    fontSize: '14px',
    boxSizing: 'border-box',
    resize: 'vertical',
  },
  error: {
    color: '#ef4444',
    fontSize: '12px',
    marginTop: '4px',
    display: 'block',
  },
  globalError: {
    color: '#ef4444',
    fontSize: '14px',
    marginBottom: '16px',
    textAlign: 'center',
  },
  success: {
    color: '#10b981',
    fontSize: '14px',
    marginBottom: '16px',
    textAlign: 'center',
  },
  button: {
    width: '100%',
    padding: '12px',
    color: '#ffffff',
    border: 'none',
    borderRadius: '6px',
    fontSize: '16px',
    fontWeight: 500,
  },
}