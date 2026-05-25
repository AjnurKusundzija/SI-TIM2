import { useState } from 'react'
import { Link } from 'react-router-dom'
import { createTicket, createTicketWithAttachments } from '../services/ticketService'
import { CheckCircle } from 'lucide-react'
import FileUpload from '../components/common/FileUpload'

const PROBLEM_CATEGORIES = [
  { value: 'INTERNET', label: 'Internet' },
  { value: 'TV', label: 'TV' },
  { value: 'MOBILE_NETWORK', label: 'Mobilna mreža' },
  { value: 'BILLING', label: 'Računi/Naplata' },
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
  const [files, setFiles] = useState([])
  const [errors, setErrors] = useState({})
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)
  const [error, setError] = useState(null)

  const validateForm = () => {
    const newErrors = {}
    if (!formData.subject.trim()) newErrors.subject = 'Naslov je obavezan.'
    if (!formData.type) newErrors.type = 'Tip problema je obavezan.'
    if (!formData.description.trim()) newErrors.description = 'Opis je obavezan.'
    if (!formData.priority) newErrors.priority = 'Prioritet je obavezan.'
    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
    if (errors[name]) setErrors((prev) => ({ ...prev, [name]: '' }))
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (!validateForm()) return

    setLoading(true)
    setError(null)
    setSuccess(false)

    try {
      const ticketData = {
        Subject: formData.subject.trim(),
        Type: formData.type,
        Description: formData.description.trim(),
        Priority: formData.priority,
      }

      // PB-56: Ako ima fajlova, koristi endpoint sa attachments-ima
      if (files.length > 0) {
        await createTicketWithAttachments(ticketData, files)
      } else {
        await createTicket(ticketData)
      }

      setSuccess(true)
      setFormData({ subject: '', type: '', description: '', priority: '' })
      setFiles([])
    } catch (err) {
      console.error(err)
      if (err.response?.status === 401) {
        setError('Niste ovlašteni. Molimo prijavite se ponovo.')
      } else if (err.response?.status === 400) {
        setError(err.response?.data?.message || 'Nevažeći podaci. Molimo provjerite unos.')
      } else {
        setError('Nije uspjelo kreiranje tiketa. Molimo pokušajte ponovo.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-xl shadow-sm border border-gray-100">
        <div className="px-6 py-5 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900">Kreiraj novi tiket</h2>
          <p className="text-sm text-gray-500 mt-1">Podnesite novi zahtjev za podršku</p>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-5">
          <div>
            <label htmlFor="subject" className="block text-sm font-medium text-gray-700 mb-1">
              Naslov <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              id="subject"
              name="subject"
              value={formData.subject}
              onChange={handleChange}
              placeholder="Kratak opis problema"
              className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none transition-colors"
            />
            {errors.subject && <p className="mt-1 text-xs text-red-600">{errors.subject}</p>}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="type" className="block text-sm font-medium text-gray-700 mb-1">
                Tip problema <span className="text-red-500">*</span>
              </label>
              <select
                id="type"
                name="type"
                value={formData.type}
                onChange={handleChange}
                className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
              >
                <option value="">Odaberi tip</option>
                {PROBLEM_CATEGORIES.map((cat) => (
                  <option key={cat.value} value={cat.value}>{cat.label}</option>
                ))}
              </select>
              {errors.type && <p className="mt-1 text-xs text-red-600">{errors.type}</p>}
            </div>

            <div>
              <label htmlFor="priority" className="block text-sm font-medium text-gray-700 mb-1">
                Prioritet <span className="text-red-500">*</span>
              </label>
              <select
                id="priority"
                name="priority"
                value={formData.priority}
                onChange={handleChange}
                className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
              >
                <option value="">Odaberi prioritet</option>
                {PRIORITIES.map((pri) => (
                  <option key={pri.value} value={pri.value}>{pri.label}</option>
                ))}
              </select>
              {errors.priority && <p className="mt-1 text-xs text-red-600">{errors.priority}</p>}
            </div>
          </div>

          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
              Opis <span className="text-red-500">*</span>
            </label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={6}
              placeholder="Detaljno opišite vaš problem..."
              className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none transition-colors"
            />
            {errors.description && <p className="mt-1 text-xs text-red-600">{errors.description}</p>}
          </div>

          {/* PB-56 / US-80: Upload attachment-ima */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Dodaj prilog (opcionalno)
            </label>
            <FileUpload onFilesSelected={setFiles} maxFiles={5} compact={false} />
          </div>

          {error && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
              {error}
            </div>
          )}

          {success && (
            <div className="p-3 bg-emerald-50 border border-emerald-200 rounded-lg text-sm text-emerald-700 flex items-center gap-2">
              <CheckCircle size={16} />
              <span>Tiket je uspješno kreiran!</span>
              <Link
                to="/mytickets"
                className="ml-auto font-medium underline underline-offset-2 hover:text-emerald-900 whitespace-nowrap"
              >
                Pogledaj tikete →
              </Link>
            </div>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => {
                setFormData({ subject: '', type: '', description: '', priority: '' })
                setFiles([])
                setErrors({})
                setError(null)
                setSuccess(false)
              }}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Poništi
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-2 text-sm font-medium text-white bg-navy-700 hover:bg-navy-800 rounded-lg transition-colors disabled:opacity-50"
            >
              {loading ? 'Kreiranje...' : 'Kreiraj tiket'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
