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
import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
    ArrowLeft,
    User,
    Tag,
    Clock,
    Send,
    XCircle,
    MessageCircle,
} from 'lucide-react'
import { getTicketById, getTicketComments } from '../services/ticketService'
import { useAuth } from '../context/AuthContext'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'

export default function TicketDetail() {
    const { id } = useParams()
    const navigate = useNavigate()
    const { user } = useAuth()

    const backPath = user?.role === 'CLIENT' ? '/mytickets' : '/tickets'

    const [ticket, setTicket] = useState(null)
    const [comments, setComments] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    // message je placeholder za SignalR — handleSend će biti spojen na Hub (PB-27)
    const [message, setMessage] = useState('')

    useEffect(() => {
        const ticketId = Number(id)

        Promise.all([
            getTicketById(ticketId),
            getTicketComments(ticketId),
        ])
            .then(([fetchedTicket, fetchedComments]) => {
                setTicket(fetchedTicket)
                setComments(fetchedComments)
            })
            .catch((err) => {
                console.error(err)
                setError('Nije moguće učitati detalje tiketa.')
            })
            .finally(() => setLoading(false))
    }, [id])

    // Slanje poruke — implementira se u PB-27 putem SignalR Hub-a
    const handleSend = () => {
        if (!message.trim()) return
        setMessage('')
    }

    // Zatvaranje tiketa — implementira se u PB-25
    const handleCloseTicket = () => {}

    if (loading) {
        return (
            <div className="flex justify-center py-16">
                <div className="w-8 h-8 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
            </div>
        )
    }

    if (error || !ticket) {
        return (
            <EmptyState
                title="Tiket nije pronađen"
                description={error || 'Traženi tiket ne postoji ili nemate pristup.'}
                action={() => navigate(backPath)}
                actionLabel="Nazad na tikete"
            />
        )
    }

    const title = ticket.title || 'Bez naslova'
    const description = ticket.description || 'Nema opisa za ovaj tiket.'
    const category = ticket.problemCategory
    const createdDate = ticket.createdDate
        ? new Date(ticket.createdDate).toLocaleString('bs-BA')
        : '—'

    const clientName = ticket.clientName || 'Klijent'
    const agentName = ticket.assignedAgentName || 'Nije dodijeljen'

    return (
        <div className="max-w-5xl mx-auto space-y-5">
            <Link
                to={backPath}
                className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-navy-700 transition-colors"
            >
                <ArrowLeft size={16} />
                Nazad na tikete
            </Link>

            {/* Detalji tiketa — US-14 */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-semibold text-gray-900">{title}</h2>
                        <p className="text-xs text-gray-400 mt-1">ticket-{ticket.ticketId}</p>
                    </div>

                    <div className="flex flex-wrap gap-2">
                        {ticket.status && <Badge value={ticket.status} />}
                        {ticket.priority && <Badge value={ticket.priority} />}
                        {category && <Badge value={category} />}
                    </div>
                </div>

                <p className="text-sm text-gray-600 leading-6 mt-5">{description}</p>

                <div className="border-t border-gray-100 mt-6 pt-4 grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-500">
                    <div className="flex items-center gap-2">
                        <User size={16} />
                        <span>Kreirao: <strong className="text-gray-700">{clientName}</strong></span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Tag size={16} />
                        <span>Agent: <strong className="text-gray-700">{agentName}</strong></span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Clock size={16} />
                        <span>Kreirano: <strong className="text-gray-700">{createdDate}</strong></span>
                    </div>
                </div>

                {ticket.status !== 'CLOSED' && (
                    <div className="border-t border-gray-100 mt-5 pt-4">
                        <button
                            type="button"
                            onClick={handleCloseTicket}
                            className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-red-700 bg-red-50 hover:bg-red-100 rounded-lg transition-colors"
                        >
                            <XCircle size={16} />
                            Zatvori tiket
                        </button>
                    </div>
                )}
            </section>

            {/* Historija komunikacije — US-15 */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100">
                <div className="px-6 py-4 border-b border-gray-100 flex items-center gap-2">
                    <MessageCircle size={18} className="text-gray-500" />
                    <h3 className="text-sm font-semibold text-gray-900">
                        Razgovor ({comments.length})
                    </h3>
                </div>

                <div className="p-6 space-y-5">
                    {comments.length === 0 ? (
                        <p className="text-sm text-gray-400 text-center py-4">
                            Nema poruka u razgovoru.
                        </p>
                    ) : (
                        comments.map((comment) => {
                            const initials = comment.authorName
                                .split(' ')
                                .map((p) => p[0])
                                .join('')
                                .slice(0, 2)
                                .toUpperCase()

                            return (
                                <div key={comment.commentId} className="flex gap-3">
                                    <div className="w-9 h-9 rounded-full bg-navy-100 text-navy-700 flex items-center justify-center text-xs font-semibold flex-shrink-0">
                                        {initials}
                                    </div>

                                    <div className="flex-1">
                                        <div className="flex flex-wrap items-center justify-between gap-2">
                                            <div className="flex items-center gap-2">
                                                <span className="text-sm font-semibold text-gray-900">
                                                    {comment.authorName}
                                                </span>
                                                {comment.authorRole && (
                                                    <Badge value={comment.authorRole} />
                                                )}
                                            </div>
                                            <span className="text-xs text-gray-400">
                                                {new Date(comment.dateTime).toLocaleString('bs-BA')}
                                            </span>
                                        </div>

                                        <p className="text-sm text-gray-600 mt-1 leading-6">
                                            {comment.content}
                                        </p>
                                    </div>
                                </div>
                            )
                        })
                    )}

                    {/* Unos poruke — spojiće se na SignalR Hub u PB-27 */}
                    {ticket.status !== 'CLOSED' && (
                        <div className="space-y-3 border-t border-gray-100 pt-4">
                            <textarea
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                rows={3}
                                placeholder="Unesite vašu poruku..."
                                className="w-full px-3 py-3 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none"
                            />

                            <div className="flex justify-end">
                                <button
                                    type="button"
                                    onClick={handleSend}
                                    disabled={!message.trim()}
                                    className="inline-flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-800 disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium rounded-lg transition-colors"
                                >
                                    <Send size={16} />
                                    Pošalji
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </section>
        </div>
    )
}
