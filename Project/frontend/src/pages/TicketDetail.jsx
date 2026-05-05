import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { getTicketById } from '../services/ticketService'
import { ArrowLeft, AlertCircle } from 'lucide-react'

const STATUS_CLASSES = {
  OPEN: 'bg-emerald-100 text-emerald-800',
  CLOSED: 'bg-gray-100 text-gray-800',
  PENDING_CLOSE: 'bg-amber-100 text-amber-800',
}

const PRIORITY_CLASSES = {
  HIGH: 'bg-red-100 text-red-800',
  MEDIUM: 'bg-yellow-100 text-yellow-800',
  LOW: 'bg-blue-100 text-blue-800',
}

const STATUS_LABELS = { OPEN: 'Otvoren', CLOSED: 'Zatvoren', PENDING_CLOSE: 'Čeka zatvaranje' }
const PRIORITY_LABELS = { LOW: 'Nizak', MEDIUM: 'Srednji', HIGH: 'Visok' }
const TYPE_LABELS = {
  INTERNET: 'Internet',
  TV: 'TV',
  MOBILE_NETWORK: 'Mobilna mreža',
  BILLING: 'Računi/Naplata',
  TECHNICAL_SUPPORT: 'Tehnička podrška',
}
const ROLE_LABELS = {
  CLIENT: 'Klijent',
  AGENT: 'Agent',
  TECHNICIAN: 'Tehničar',
  ADMINISTRATOR: 'Administrator',
}

function formatDateTime(dateStr) {
  if (!dateStr) return '—'
  return new Date(dateStr).toLocaleString('bs-BA', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  })
}

function Initials({ name }) {
  const parts = name?.trim().split(' ') ?? []
  const initials = parts.length >= 2
    ? parts[0][0] + parts[parts.length - 1][0]
    : (parts[0]?.[0] ?? '?')
  return (
    <div className="w-9 h-9 rounded-full bg-navy-600 flex items-center justify-center text-white text-sm font-medium flex-shrink-0">
      {initials.toUpperCase()}
    </div>
  )
}

export default function TicketDetail() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [ticket, setTicket] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState(null)

  useEffect(() => {
    getTicketById(id)
      .then(setTicket)
      .catch((err) => {
        console.error(err)
        if (err.response?.status === 404) {
          setError('Tiket nije pronađen.')
        } else {
          setError('Greška pri učitavanju tiketa.')
        }
      })
      .finally(() => setLoading(false))
  }, [id])

  if (loading) {
    return (
      <div className="flex justify-center py-24">
        <div className="w-8 h-8 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
      </div>
    )
  }

  if (error) {
    return (
      <div className="space-y-4">
        <button
          onClick={() => navigate(-1)}
          className="flex items-center gap-2 text-sm text-gray-500 hover:text-gray-800 transition-colors"
        >
          <ArrowLeft size={16} /> Nazad na tikete
        </button>
        <div className="flex items-center gap-3 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700">
          <AlertCircle size={18} />
          <span className="text-sm">{error}</span>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-5 max-w-3xl">
      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-gray-800 transition-colors"
      >
        <ArrowLeft size={16} /> Nazad na tikete
      </button>

      {/* Ticket card */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-5">
        {/* Header */}
        <div className="flex flex-wrap items-start justify-between gap-3">
          <div>
            <h2 className="text-lg font-semibold text-gray-900">{ticket.title}</h2>
            <p className="text-xs text-gray-400 mt-0.5">tiket-{ticket.ticketId}</p>
          </div>
          <div className="flex flex-wrap gap-2">
            <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium ${STATUS_CLASSES[ticket.status] ?? 'bg-gray-100 text-gray-800'}`}>
              {STATUS_LABELS[ticket.status] ?? ticket.status}
            </span>
            <span className={`inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium ${PRIORITY_CLASSES[ticket.priority] ?? 'bg-gray-100 text-gray-800'}`}>
              {PRIORITY_LABELS[ticket.priority] ?? ticket.priority}
            </span>
            <span className="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-indigo-100 text-indigo-800">
              {TYPE_LABELS[ticket.problemCategory] ?? ticket.problemCategory}
            </span>
          </div>
        </div>

        {/* Meta */}
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4 text-sm border-t border-gray-50 pt-4">
          <div>
            <p className="text-xs text-gray-400 mb-0.5">Kreirao</p>
            <p className="font-medium text-gray-800">{ticket.creatorName}</p>
          </div>
          <div>
            <p className="text-xs text-gray-400 mb-0.5">Datum kreiranja</p>
            <p className="font-medium text-gray-800">{formatDateTime(ticket.createdDate)}</p>
          </div>
          {ticket.closedDate && (
            <div>
              <p className="text-xs text-gray-400 mb-0.5">Datum zatvaranja</p>
              <p className="font-medium text-gray-800">{formatDateTime(ticket.closedDate)}</p>
            </div>
          )}
        </div>

        {/* Description */}
        <div className="border-t border-gray-50 pt-4">
          <p className="text-xs text-gray-400 mb-1.5">Opis</p>
          <p className="text-sm text-gray-700 whitespace-pre-wrap leading-relaxed">{ticket.description}</p>
        </div>
      </div>

      {/* Conversation */}
      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
        <h3 className="text-sm font-semibold text-gray-700 mb-4">
          Razgovor ({ticket.comments?.length ?? 0})
        </h3>

        {ticket.comments?.length === 0 ? (
          <p className="text-sm text-gray-400 text-center py-6">Nema komentara za ovaj tiket.</p>
        ) : (
          <div className="space-y-4">
            {ticket.comments.map((c) => (
              <div key={c.commentId} className="flex gap-3">
                <Initials name={c.authorName} />
                <div className="flex-1 min-w-0">
                  <div className="flex flex-wrap items-baseline gap-2 mb-1">
                    <span className="text-sm font-medium text-gray-900">{c.authorName}</span>
                    <span className="text-xs text-gray-400 bg-gray-100 px-1.5 py-0.5 rounded">
                      {ROLE_LABELS[c.authorRole] ?? c.authorRole}
                    </span>
                    <span className="text-xs text-gray-400">{formatDateTime(c.dateTime)}</span>
                    {c.isInternal && (
                      <span className="text-xs text-amber-700 bg-amber-100 px-1.5 py-0.5 rounded">Interni</span>
                    )}
                  </div>
                  <p className="text-sm text-gray-700 whitespace-pre-wrap leading-relaxed">{c.content}</p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
