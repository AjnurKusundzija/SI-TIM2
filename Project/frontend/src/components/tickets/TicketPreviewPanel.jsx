import PropTypes from 'prop-types'
import { useState, useEffect, useRef } from 'react'
import { Link } from 'react-router-dom'
import * as signalR from '@microsoft/signalr'
import {
    AlertCircle,
    ArrowUpRight,
    Clock,
    Info,
    Send,
    X,
} from 'lucide-react'
import { getTicketById, getTicketComments, addComment } from '../../services/ticketService'
import { useAuth } from '../../context/AuthContext'
import Badge from '../common/Badge'
import { formatDateTime, formatDateOnly } from '../../utils/formatDate'

const MAX_COMMENT_LENGTH = 1000

const TYPE_LABELS = {
    INTERNET: 'Internet',
    TV: 'TV',
    MOBILE_NETWORK: 'Mobilna mreža',
    BILLING: 'Računi/Naplata',
    TECHNICAL_SUPPORT: 'Tehnička podrška',
}

function initials(name) {
    if (!name) return '?'
    const parts = name.trim().split(' ')
    return (parts.length >= 2 ? `${parts[0][0]}${parts[parts.length - 1][0]}` : name.slice(0, 2)).toUpperCase()
}

function Avatar({ name, role }) {
    const colors = {
        CLIENT: 'bg-sky-100 text-sky-700 ring-sky-200',
        AGENT: 'bg-violet-100 text-violet-700 ring-violet-200',
        TECHNICIAN: 'bg-lime-100 text-lime-700 ring-lime-200',
        ADMINISTRATOR: 'bg-rose-100 text-rose-700 ring-rose-200',
    }
    const cls = colors[role] || 'bg-gray-100 text-gray-600 ring-gray-200'
    return (
        <div className={`w-8 h-8 rounded-full flex items-center justify-center text-[11px] font-bold flex-shrink-0 ring-1 ${cls}`}>
            {initials(name)}
        </div>
    )
}

Avatar.propTypes = { name: PropTypes.string, role: PropTypes.string }

// ─── Loading skeleton ─────────────────────────────────────────────────────────

function PanelSkeleton({ onClose }) {
    return (
        <div className="flex flex-col h-full">
            <div className="flex-shrink-0 flex items-center justify-between px-5 py-4 border-b border-gray-100">
                <div className="space-y-1.5 flex-1 animate-pulse">
                    <div className="h-4 bg-gray-200 rounded w-2/3" />
                    <div className="h-3 bg-gray-100 rounded w-24" />
                </div>
                <button onClick={onClose} className="text-gray-400 hover:text-gray-600 ml-4">
                    <X size={18} />
                </button>
            </div>
            <div className="flex-1 px-5 py-5 space-y-5 animate-pulse overflow-hidden">
                <div className="grid grid-cols-2 gap-3">
                    {[0,1,2,3].map(i => <div key={i} className="h-10 bg-gray-100 rounded-lg" />)}
                </div>
                <div className="border-t border-gray-100 pt-5 space-y-4">
                    {[0,1,2].map(i => (
                        <div key={i} className="flex gap-3">
                            <div className="w-8 h-8 rounded-full bg-gray-200 flex-shrink-0" />
                            <div className="flex-1 space-y-1.5">
                                <div className="h-3 bg-gray-200 rounded w-28" />
                                <div className="h-3 bg-gray-100 rounded w-full" />
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    )
}

PanelSkeleton.propTypes = { onClose: PropTypes.func.isRequired }

// ─── Main component ───────────────────────────────────────────────────────────

export default function TicketPreviewPanel({ ticketId, onClose }) {
    const { user } = useAuth()
    const [ticket, setTicket] = useState(null)
    const [comments, setComments] = useState([])
    const [loading, setLoading] = useState(true)
    const [message, setMessage] = useState('')
    const [isSending, setIsSending] = useState(false)
    const [sendError, setSendError] = useState(null)
    const commentsEndRef = useRef(null)
    const prevCommentsLen = useRef(0)

    useEffect(() => {
        if (!ticketId) return
        const fetchData = async () => {
            setLoading(true)
            setTicket(null)
            setComments([])
            setMessage('')
            setSendError(null)
            try {
                const [t, c] = await Promise.all([
                    getTicketById(ticketId),
                    getTicketComments(ticketId),
                ])
                setTicket(t)
                setComments(c)
            } finally {
                setLoading(false)
            }
        }
        fetchData()
    }, [ticketId])

    useEffect(() => {
        if (!ticketId) return
        const conn = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build()

        conn.start()
            .then(() => {
                conn.invoke('JoinTicketGroup', String(ticketId)).catch(console.error)
                conn.on('ReceiveComment', c => setComments(prev => [...prev, c]))
            })
            .catch(console.error)

        return () => {
            if (conn.state === signalR.HubConnectionState.Connected) {
                conn.invoke('LeaveTicketGroup', String(ticketId)).catch(console.error)
            }
            conn.stop()
        }
    }, [ticketId])

    useEffect(() => {
        // Scroll to bottom only when a new real-time comment arrives,
        // not on initial load (prevCommentsLen is 0 after reset).
        if (comments.length > prevCommentsLen.current && prevCommentsLen.current > 0) {
            commentsEndRef.current?.scrollIntoView({ behavior: 'smooth' })
        }
        prevCommentsLen.current = comments.length
    }, [comments])

    const handleSend = async () => {
        if (!message.trim() || isSending) return
        setSendError(null)
        setIsSending(true)
        try {
            await addComment(ticketId, message)
            setMessage('')
        } catch (err) {
            setSendError(err.response?.data || 'Neuspješno slanje.')
        } finally {
            setIsSending(false)
        }
    }

    if (loading) return <PanelSkeleton onClose={onClose} />

    if (!ticket) {
        return (
            <div className="flex flex-col h-full">
                <div className="flex-shrink-0 flex justify-end px-5 py-4 border-b border-gray-100">
                    <button onClick={onClose} className="text-gray-400 hover:text-gray-600">
                        <X size={18} />
                    </button>
                </div>
                <div className="flex-1 flex items-center justify-center text-sm text-gray-400">
                    Tiket nije pronađen.
                </div>
            </div>
        )
    }

    const canReply = ticket.status !== 'CLOSED' && (
        user?.role === 'AGENT' || user?.role === 'TECHNICIAN' || user?.role === 'ADMINISTRATOR'
    )

    const allComments = [
        {
            commentId: 'initial',
            authorName: ticket.clientName || 'Klijent',
            authorRole: 'CLIENT',
            dateTime: ticket.createdDate,
            content: ticket.description || 'Nema opisa.',
        },
        ...comments,
    ]

    const metaItems = [
        { label: 'Klijent', value: ticket.clientName || '—' },
        { label: 'Agent', value: ticket.assignedAgentName || 'Nije dodijeljen' },
        ...(ticket.assignedTechnicianName ? [{ label: 'Tehničar', value: ticket.assignedTechnicianName }] : []),
        { label: 'Kategorija', value: TYPE_LABELS[ticket.problemCategory] ?? ticket.problemCategory ?? '—' },
        { label: 'Kreirano', value: ticket.createdDate ? formatDateOnly(ticket.createdDate) : '—' },
    ]

    return (
        /*
         * IMPORTANT: parent must have a defined height (e.g. h-full on a fixed/sized container)
         * so that flex-1 + overflow-y-auto on the conversation section works correctly.
         */
        <div className="flex flex-col h-full bg-white">

            {/* ── Window title bar ──────────────────────────────────────────── */}
            <div className="flex-shrink-0 px-5 pt-4 pb-3 border-b border-gray-100">
                <div className="flex items-start gap-3">
                    <div className="flex-1 min-w-0">
                        <h2 className="text-[15px] font-semibold text-gray-900 leading-snug">
                            {ticket.title || 'Bez naslova'}
                        </h2>
                        <p className="text-[11px] text-gray-400 mt-0.5 font-mono">
                            #{ticket.ticketId}
                        </p>
                    </div>
                    <div className="flex items-center gap-2 flex-shrink-0 pt-0.5">
                        <Link
                            to={`/tickets/${ticket.ticketId}`}
                            className="flex items-center gap-1 text-[11px] font-medium text-navy-600 hover:text-navy-800 transition-colors"
                            title="Otvori puni prikaz"
                        >
                            <ArrowUpRight size={14} />
                            Puni prikaz
                        </Link>
                        <div className="w-px h-4 bg-gray-200" />
                        <button
                            onClick={onClose}
                            className="text-gray-400 hover:text-gray-700 transition-colors"
                            aria-label="Zatvori"
                        >
                            <X size={17} />
                        </button>
                    </div>
                </div>

                {/* Badges row */}
                <div className="flex flex-wrap gap-1.5 mt-2.5">
                    {ticket.status && <Badge value={ticket.status} />}
                    {ticket.internalPriority
                        ? <Badge value={ticket.internalPriority} />
                        : ticket.priority && <Badge value={ticket.priority} />}
                    {ticket.problemCategory && <Badge value={ticket.problemCategory} />}
                </div>
            </div>

            {/* ── Metadata grid ─────────────────────────────────────────────── */}
            <div className="flex-shrink-0 px-5 py-3 border-b border-gray-100 bg-gray-50">
                <dl className="grid grid-cols-2 gap-x-6 gap-y-2">
                    {metaItems.map(({ label, value }) => (
                        <div key={label}>
                            <dt className="text-[10px] font-semibold uppercase tracking-wider text-gray-400">
                                {label}
                            </dt>
                            <dd className="text-xs text-gray-800 font-medium truncate mt-0.5">
                                {value}
                            </dd>
                        </div>
                    ))}
                </dl>
            </div>

            {/* ── Conversation — ONLY this section scrolls ───────────────────── */}
            <div className="flex-1 overflow-y-auto px-5 py-4 space-y-5">
                {allComments.map(comment => {
                    if (comment.isSystemMessage) {
                        return (
                            <div key={comment.commentId} className="flex items-center gap-3">
                                <div className="flex-1 border-t border-gray-100" />
                                <span className="flex items-center gap-1 text-[10px] text-gray-400 bg-white border border-gray-200 rounded-full px-2.5 py-1 whitespace-nowrap">
                                    <Info size={9} />
                                    {comment.content}
                                </span>
                                <div className="flex-1 border-t border-gray-100" />
                            </div>
                        )
                    }

                    return (
                        <div key={comment.commentId} className="flex gap-3">
                            <Avatar name={comment.authorName} role={comment.authorRole} />
                            <div className="flex-1 min-w-0">
                                <div className="flex items-baseline justify-between gap-3 mb-1">
                                    <span className="text-xs font-semibold text-gray-900 truncate">
                                        {comment.authorName}
                                    </span>
                                    <span className="text-[10px] text-gray-400 flex-shrink-0 flex items-center gap-1">
                                        <Clock size={9} />
                                        {formatDateTime(comment.dateTime)}
                                    </span>
                                </div>
                                <p className="text-sm text-gray-600 leading-relaxed break-words">
                                    {comment.content}
                                </p>
                            </div>
                        </div>
                    )
                })}
                <div ref={commentsEndRef} />
            </div>

            {/* ── Reply footer ───────────────────────────────────────────────── */}
            {canReply && (
                <div className="flex-shrink-0 border-t border-gray-100 bg-white">
                    {sendError && (
                        <div className="flex items-center gap-1.5 mx-5 mt-3 text-xs text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2">
                            <AlertCircle size={12} />
                            {sendError}
                        </div>
                    )}
                    <div className="p-4 space-y-2">
                        <textarea
                            value={message}
                            onChange={e => {
                                if (e.target.value.length <= MAX_COMMENT_LENGTH) {
                                    setMessage(e.target.value)
                                    if (sendError) setSendError(null)
                                }
                            }}
                            onKeyDown={e => {
                                if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) handleSend()
                            }}
                            rows={2}
                            placeholder="Unesite odgovor… (Ctrl+Enter za slanje)"
                            className="w-full px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none bg-gray-50 focus:bg-white transition-colors"
                        />
                        <div className="flex items-center justify-between">
                            <span className={`text-[11px] ${message.length >= MAX_COMMENT_LENGTH ? 'text-red-500 font-medium' : 'text-gray-400'}`}>
                                {message.length}/{MAX_COMMENT_LENGTH}
                            </span>
                            <div className="flex items-center gap-3">
                                <Link
                                    to={`/tickets/${ticketId}`}
                                    className="text-[11px] text-gray-400 hover:text-navy-600 transition-colors"
                                >
                                    Prosljeđivanje i zatvaranje →
                                </Link>
                                <button
                                    type="button"
                                    onClick={handleSend}
                                    disabled={!message.trim() || isSending}
                                    className="inline-flex items-center gap-1.5 px-3 py-1.5 bg-navy-700 hover:bg-navy-800 disabled:opacity-40 disabled:cursor-not-allowed text-white text-xs font-medium rounded-lg transition-colors"
                                >
                                    <Send size={12} />
                                    {isSending ? 'Slanje…' : 'Pošalji'}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

            {ticket.status === 'CLOSED' && (
                <div className="flex-shrink-0 border-t border-gray-100 bg-gray-50 px-5 py-3 text-center">
                    <p className="text-xs text-gray-400">Tiket je zatvoren.</p>
                    <Link to={`/tickets/${ticketId}`} className="text-xs text-navy-600 hover:text-navy-800 font-medium">
                        Pogledaj ocjenu i historiju →
                    </Link>
                </div>
            )}
        </div>
    )
}

TicketPreviewPanel.propTypes = {
    ticketId: PropTypes.number,
    onClose: PropTypes.func.isRequired,
}
