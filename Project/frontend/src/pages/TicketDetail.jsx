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
import { getMyTickets } from '../services/ticketService'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'

const CATEGORY_LABELS = {
    INTERNET: 'Internet',
    TV: 'TV',
    MOBILE_NETWORK: 'Mobile Network',
    BILLING: 'Billing',
    TECHNICAL_SUPPORT: 'Technical Support',
}

export default function TicketDetail() {
    const { id } = useParams()
    const navigate = useNavigate()

    const [ticket, setTicket] = useState(null)
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)
    const [message, setMessage] = useState('')

    useEffect(() => {
        getMyTickets()
            .then((tickets) => {
                const foundTicket = tickets.find(
                    (t) => String(t.ticketId) === String(id)
                )

                if (!foundTicket) {
                    setError('Tiket nije pronađen.')
                    return
                }

                setTicket(foundTicket)
            })
            .catch((err) => {
                console.error(err)
                setError('Nije moguće učitati detalje tiketa.')
            })
            .finally(() => setLoading(false))
    }, [id])

    const handleSend = () => {
        if (!message.trim()) return
        alert('Slanje poruke još nije spojeno na backend.')
        setMessage('')
    }

    const handleCloseTicket = () => {
        alert('Zatvaranje tiketa još nije spojeno na backend.')
    }

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
                action={() => navigate('/mytickets')}
                actionLabel="Nazad na tikete"
            />
        )
    }

    const title = ticket.title || ticket.subject || 'Bez naslova'
    const description = ticket.description || 'Nema opisa za ovaj tiket.'
    const category = ticket.problemCategory || ticket.type
    const createdDate = ticket.createdDate
        ? new Date(ticket.createdDate).toLocaleString()
        : '—'

    const clientName =
        ticket.clientName ||
        ticket.createdBy ||
        ticket.userFullName ||
        'Klijent'

    const agentName =
        ticket.agentName ||
        ticket.assignedAgentName ||
        ticket.assignedTo ||
        'Nije dodijeljen'

    return (
        <div className="max-w-5xl mx-auto space-y-5">
            <Link
                to="/mytickets"
                className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-navy-700 transition-colors"
            >
                <ArrowLeft size={16} />
                Back to tickets
            </Link>

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

                <p className="text-sm text-gray-600 leading-6 mt-5">
                    {description}
                </p>

                <div className="border-t border-gray-100 mt-6 pt-4 grid grid-cols-1 md:grid-cols-3 gap-4 text-sm text-gray-500">
                    <div className="flex items-center gap-2">
                        <User size={16} />
                        <span>Created by: <strong className="text-gray-700">{clientName}</strong></span>
                    </div>

                    <div className="flex items-center gap-2">
                        <Tag size={16} />
                        <span>Agent: <strong className="text-gray-700">{agentName}</strong></span>
                    </div>

                    <div className="flex items-center gap-2">
                        <Clock size={16} />
                        <span>Created: <strong className="text-gray-700">{createdDate}</strong></span>
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
                            Close Ticket
                        </button>
                    </div>
                )}
            </section>

            <section className="bg-white rounded-xl shadow-sm border border-gray-100">
                <div className="px-6 py-4 border-b border-gray-100 flex items-center gap-2">
                    <MessageCircle size={18} className="text-gray-500" />
                    <h3 className="text-sm font-semibold text-gray-900">
                        Conversation (1)
                    </h3>
                </div>

                <div className="p-6 space-y-5">
                    <div className="flex gap-3">
                        <div className="w-9 h-9 rounded-full bg-navy-100 text-navy-700 flex items-center justify-center text-xs font-semibold">
                            {clientName
                                .split(' ')
                                .map((part) => part[0])
                                .join('')
                                .slice(0, 2)
                                .toUpperCase()}
                        </div>

                        <div className="flex-1">
                            <div className="flex flex-wrap items-center justify-between gap-2">
                                <div className="flex items-center gap-2">
                                    <span className="text-sm font-semibold text-gray-900">
                                        {clientName}
                                    </span>
                                    <Badge value="CLIENT" />
                                </div>
                                <span className="text-xs text-gray-400">{createdDate}</span>
                            </div>

                            <p className="text-sm text-gray-600 mt-2 leading-6">
                                {description}
                            </p>
                        </div>
                    </div>

                    {ticket.status !== 'CLOSED' && (
                        <div className="space-y-3">
                            <textarea
                                value={message}
                                onChange={(e) => setMessage(e.target.value)}
                                rows={3}
                                placeholder="Type your message..."
                                className="w-full px-3 py-3 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none"
                            />

                            <div className="flex justify-end">
                                <button
                                    type="button"
                                    onClick={handleSend}
                                    className="inline-flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium rounded-lg transition-colors"
                                >
                                    <Send size={16} />
                                    Send
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </section>
        </div>
    )
}