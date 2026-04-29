// PB-22: Kreiraj novi tiket
import { useState } from 'react'
import { createTicket } from '../services/ticketService'
import { CheckCircle } from 'lucide-react'

const PROBLEM_CATEGORIES = [
  { value: 'INTERNET', label: 'Internet' },
  { value: 'TV', label: 'TV' },
  { value: 'MOBILE_NETWORK', label: 'Mobile Network' },
  { value: 'BILLING', label: 'Billing' },
  { value: 'TECHNICAL_SUPPORT', label: 'Technical Support' },
]

const PRIORITIES = [
  { value: 'LOW', label: 'Low' },
  { value: 'MEDIUM', label: 'Medium' },
  { value: 'HIGH', label: 'High' },
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
    if (!formData.subject.trim()) newErrors.subject = 'Subject is required.'
    if (!formData.type) newErrors.type = 'Problem type is required.'
    if (!formData.description.trim()) newErrors.description = 'Description is required.'
    if (!formData.priority) newErrors.priority = 'Priority is required.'
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
      await createTicket({
        Subject: formData.subject.trim(),
        Type: formData.type,
        Description: formData.description.trim(),
        Priority: formData.priority,
      })
      setSuccess(true)
      setFormData({ subject: '', type: '', description: '', priority: '' })
    } catch (err) {
      console.error(err)
      if (err.response?.status === 401) {
        setError('Not authorized. Please sign in again.')
      } else if (err.response?.status === 400) {
        setError('Invalid data. Please check your input.')
      } else {
        setError('Failed to create ticket. Please try again.')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-xl shadow-sm border border-gray-100">
        <div className="px-6 py-5 border-b border-gray-100">
          <h2 className="text-lg font-semibold text-gray-900">Create New Ticket</h2>
          <p className="text-sm text-gray-500 mt-1">Submit a new support request</p>
        </div>

        <form onSubmit={handleSubmit} className="p-6 space-y-5">
          <div>
            <label htmlFor="subject" className="block text-sm font-medium text-gray-700 mb-1">
              Subject <span className="text-red-500">*</span>
            </label>
            <input
              type="text"
              id="subject"
              name="subject"
              value={formData.subject}
              onChange={handleChange}
              placeholder="Brief description of the issue"
              className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none transition-colors"
            />
            {errors.subject && <p className="mt-1 text-xs text-red-600">{errors.subject}</p>}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label htmlFor="type" className="block text-sm font-medium text-gray-700 mb-1">
                Problem Type <span className="text-red-500">*</span>
              </label>
              <select
                id="type"
                name="type"
                value={formData.type}
                onChange={handleChange}
                className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
              >
                <option value="">Select type</option>
                {PROBLEM_CATEGORIES.map((cat) => (
                  <option key={cat.value} value={cat.value}>{cat.label}</option>
                ))}
              </select>
              {errors.type && <p className="mt-1 text-xs text-red-600">{errors.type}</p>}
            </div>

            <div>
              <label htmlFor="priority" className="block text-sm font-medium text-gray-700 mb-1">
                Priority <span className="text-red-500">*</span>
              </label>
              <select
                id="priority"
                name="priority"
                value={formData.priority}
                onChange={handleChange}
                className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm bg-white focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none"
              >
                <option value="">Select priority</option>
                {PRIORITIES.map((pri) => (
                  <option key={pri.value} value={pri.value}>{pri.label}</option>
                ))}
              </select>
              {errors.priority && <p className="mt-1 text-xs text-red-600">{errors.priority}</p>}
            </div>
          </div>

          <div>
            <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
              Description <span className="text-red-500">*</span>
            </label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows={6}
              placeholder="Describe your issue in detail..."
              className="w-full px-3 py-2.5 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none transition-colors"
            />
            {errors.description && <p className="mt-1 text-xs text-red-600">{errors.description}</p>}
          </div>

          {error && (
            <div className="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-700">
              {error}
            </div>
          )}

          {success && (
            <div className="p-3 bg-emerald-50 border border-emerald-200 rounded-lg text-sm text-emerald-700 flex items-center gap-2">
              <CheckCircle size={16} />
              Ticket created successfully!
            </div>
          )}

          <div className="flex justify-end gap-3 pt-2">
            <button
              type="button"
              onClick={() => {
                setFormData({ subject: '', type: '', description: '', priority: '' })
                setErrors({})
                setError(null)
                setSuccess(false)
              }}
              className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
            >
              Reset
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-6 py-2 text-sm font-medium text-white bg-navy-700 hover:bg-navy-800 rounded-lg transition-colors disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Ticket'}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
