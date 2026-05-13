import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import {
    ArrowLeft,
    ArrowRightLeft,
    CheckCircle,
    Clock,
    MessageCircle,
    Send,
    Tag,
    User,
    Users,
    Wrench,
    XCircle,
    Zap,
} from 'lucide-react'
import * as signalR from '@microsoft/signalr'
import {
    addComment,
    getTicketById,
    autoForwardTicket,
    forwardTicketToAgent,
    getAgentScores,
    getTicketComments,
    forwardTicketToTechnician,
    updateInternalPriority,
    closeTicket,
    requestTicketClosure,
    acceptTicketClosure,
    rejectTicketClosure,
    forceCloseTicket,
} from '../services/ticketService'
import { useAuth } from '../context/AuthContext'
import Badge from '../components/common/Badge'
import EmptyState from '../components/common/EmptyState'
import Modal from '../components/common/Modal'

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
    const [isSending, setIsSending] = useState(false)

    // US-55, US-56: Stanje modalnog prozora za prosljeđivanje
    const [forwardModalOpen, setForwardModalOpen] = useState(false)
    const [forwardStep, setForwardStep] = useState('choice') // 'choice' | 'agents' | 'success'
    const [agentScores, setAgentScores] = useState([])
    const [selectedAgent, setSelectedAgent] = useState(null)
    const [forwardLoading, setForwardLoading] = useState(false)
    const [forwardError, setForwardError] = useState(null)
    const [forwardedTo, setForwardedTo] = useState(null)

    // Closure Workflow States
    const [closureNotification, setClosureNotification] = useState(null)
    const [timeLeft, setTimeLeft] = useState('')

    const INTERNAL_PRIORITIES = [
        { value: 1, key: 'LOW', label: 'Nizak' },
        { value: 2, key: 'MEDIUM', label: 'Srednji' },
        { value: 3, key: 'HIGH', label: 'Visok' },
        { value: 4, key: 'CRITICAL', label: 'Kritičan' },
    ]

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

    useEffect(() => {
        if (ticket?.status !== 'CLOSURE_REQUESTED') {
            setTimeLeft('')
            return
        }

        const updateTimer = () => {
            const clientComments = comments.filter(c => c.authorRole === 'CLIENT')
            const lastCommentDate = clientComments.length > 0
                ? new Date(Math.max(...clientComments.map(c => new Date(c.dateTime))))
                : new Date(ticket.createdDate)

            const expireDate = new Date(lastCommentDate.getTime() + 7 * 24 * 60 * 60 * 1000)
            const now = new Date()
            const diff = expireDate - now

            if (diff <= 0) {
                setTimeLeft('Može se prisilno zatvoriti')
                return
            }

            const days = Math.floor(diff / (1000 * 60 * 60 * 24))
            const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
            const minutes = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60))

            setTimeLeft(`Preostalo: ${days}d ${hours}h ${minutes}m`)
        }

        updateTimer()
        const interval = setInterval(updateTimer, 60000)
        return () => clearInterval(interval)
    }, [ticket, comments])

    useEffect(() => {
        if (!id) return;

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build();

        newConnection.start()
            .then(() => {
                newConnection.invoke('JoinTicketGroup', id).catch(e => console.error(e));
                newConnection.on('ReceiveComment', (comment) => {
                    setComments((prev) => [...prev, comment]);
                });
            })
            .catch(e => console.error('SignalR Connection Error: ', e));

        return () => {
            if (newConnection.state === signalR.HubConnectionState.Connected) {
                newConnection.invoke('LeaveTicketGroup', id).catch(console.error);
            }
            newConnection.stop();
        };
    }, [id]);

    // Slanje poruke — implementira se u PB-27 putem SignalR Hub-a
    const handleSend = async () => {
        if (!message.trim() || isSending) return
        setIsSending(true)
        try {
            await addComment(id, message)
            setMessage('')
        } catch (err) {
            console.error('Failed to send comment', err)
            alert(err.response?.data || 'Neuspješno slanje poruke.')
        } finally {
            setIsSending(false)
        }
    }

    // Zatvaranje tiketa — implementira se u PB-25
    //const handleCloseTicket = () => { }

    // US-55, US-56: Otvaranje modalnog prozora za prosljeđivanje
    const handleOpenForward = () => {
        setForwardModalOpen(true)
        setForwardStep('choice')
        setSelectedAgent(null)
        setForwardError(null)
        setForwardedTo(null)
    }

    const handleCloseForward = () => {
        setForwardModalOpen(false)
        setForwardStep('choice')
        setAgentScores([])
        setSelectedAgent(null)
        setForwardError(null)
        setForwardedTo(null)
    }

    // US-55: Automatsko prosljeđivanje agentu s najvišim score-om
    const handleAutoForward = async () => {
        setForwardLoading(true)
        setForwardError(null)
        try {
            const result = await autoForwardTicket(Number(id))
            setForwardedTo(result)
            setForwardStep('success')
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
        } catch (err) {
            setForwardError(err.response?.data?.poruka || 'Prosljeđivanje nije uspješno.')
        } finally {
            setForwardLoading(false)
        }
    }

    // US-56: Učitavanje liste agenata sa score-ovima
    const handleShowAgents = async () => {
        setForwardLoading(true)
        setForwardError(null)
        try {
            const scores = await getAgentScores(Number(id))
            setAgentScores(scores)
            setForwardStep('agents')
        } catch (err) {
            setForwardError(err.response?.data?.poruka || 'Nije moguće učitati listu agenata.')
        } finally {
            setForwardLoading(false)
        }
    }

    // US-56: Prosljeđivanje odabranom agentu
    const handleForwardToAgent = async () => {
        if (!selectedAgent) return
        setForwardLoading(true)
        setForwardError(null)
        try {
            const result = await forwardTicketToAgent(Number(id), selectedAgent.userId)
            setForwardedTo(result)
            setForwardStep('success')
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
        } catch (err) {
            setForwardError(err.response?.data?.poruka || 'Prosljeđivanje nije uspješno.')
        } finally {
            setForwardLoading(false)
        }
    }

    // US-TechnicianForwarding: Automatsko prosljeđivanje tehničaru na lokaciji kreatora
    const handleForwardToTechnician = async () => {
        setForwardLoading(true)
        setForwardError(null)
        try {
            const result = await forwardTicketToTechnician(Number(id))
            setForwardedTo(result)
            setForwardStep('success')
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
        } catch (err) {
            setForwardError(err.response?.data?.poruka || 'Prosljeđivanje nije uspješno.')
        } finally {
            setForwardLoading(false)
        }
    }

    // Internal Priority Management
    const [updatingPriority, setUpdatingPriority] = useState(false)
    const [priorityNotification, setPriorityNotification] = useState(null) // { type: 'success' | 'error', message: string }

    const handleUpdateInternalPriority = async (newPriorityValue) => {
        setUpdatingPriority(true)
        setPriorityNotification(null)
        try {
            await updateInternalPriority(Number(id), newPriorityValue)
            setPriorityNotification({ type: 'success', message: 'Interni prioritet je uspješno ažuriran.' })
            // Osvježi tiket
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
        } catch (err) {
            console.error('Failed to update internal priority', err)
            setPriorityNotification({ type: 'error', message: err.response?.data?.message || 'Greška pri ažuriranju prioriteta.' })
        } finally {
            setUpdatingPriority(false)
            // Sakrij notifikaciju nakon 3 sekunde
            setTimeout(() => setPriorityNotification(null), 3000)
        }
    }

    // Closure Workflow Actions
    const [closureLoading, setClosureLoading] = useState(false)

    const handleCloseTicketAction = async () => {
        if (!window.confirm('Jeste li sigurni da želite zatvoriti ovaj tiket?')) return
        setClosureLoading(true)
        try {
            await closeTicket(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Tiket je uspješno zatvoren.' })
        } catch (err) {
            console.error(err)
            setClosureNotification({ type: 'error', message: err.response?.data?.poruka || 'Greška pri zatvaranju tiketa.' })
        } finally {
            setClosureLoading(false)
            setTimeout(() => setClosureNotification(null), 3000)
        }
    }

    const handleRequestClosure = async () => {
        setClosureLoading(true)
        try {
            await requestTicketClosure(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Zahtjev za zatvaranje je poslan klijentu.' })
        } catch (err) {
            console.error(err)
            setClosureNotification({ type: 'error', message: err.response?.data?.poruka || 'Greška pri slanju zahtjeva.' })
        } finally {
            setClosureLoading(false)
            setTimeout(() => setClosureNotification(null), 3000)
        }
    }

    const handleAcceptClosure = async () => {
        setClosureLoading(true)
        try {
            await acceptTicketClosure(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Zatvaranje tiketa je prihvaćeno.' })
        } catch (err) {
            console.error(err)
            setClosureNotification({ type: 'error', message: err.response?.data?.poruka || 'Greška pri prihvaćanju zahtjeva.' })
        } finally {
            setClosureLoading(false)
            setTimeout(() => setClosureNotification(null), 3000)
        }
    }

    const handleRejectClosure = async () => {
        setClosureLoading(true)
        try {
            await rejectTicketClosure(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Zatvaranje tiketa je odbijeno.' })
        } catch (err) {
            console.error(err)
            setClosureNotification({ type: 'error', message: err.response?.data?.poruka || 'Greška pri odbijanju zahtjeva.' })
        } finally {
            setClosureLoading(false)
            setTimeout(() => setClosureNotification(null), 3000)
        }
    }

    const handleForceClose = async () => {
        setClosureLoading(true)
        try {
            await forceCloseTicket(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Tiket je prisilno zatvoren.' })
        } catch (err) {
            console.error(err)
            setClosureNotification({ type: 'error', message: err.response?.data?.poruka || 'Greška pri prisilnom zatvaranju.' })
        } finally {
            setClosureLoading(false)
            setTimeout(() => setClosureNotification(null), 3000)
        }
    }

    const forwardModalTitle = {
        choice: 'Proslijedi tiket',
        agents: 'Odaberi agenta',
        locations: 'Odaberi lokaciju',
        success: 'Tiket proslijeđen',
    }[forwardStep]

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
    const category = ticket.problemCategory
    const createdDate = ticket.createdDate
        ? new Date(ticket.createdDate).toLocaleString('bs-BA')
        : '—'

    const clientName = ticket.clientName || 'Klijent'
    const agentName = ticket.assignedAgentName || 'Nije dodijeljen'

    const initialComment = {
        commentId: 'initial',
        authorName: clientName,
        authorRole: 'CLIENT',
        dateTime: ticket.createdDate,
        content: ticket.description || 'Nema opisa za ovaj tiket.'
    }
    const allComments = [initialComment, ...comments]

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
                    <div className="border-t border-gray-100 mt-5 pt-4 flex flex-wrap gap-2">
                        {/* Akcije za Klijenta */}
                        {user?.role === 'CLIENT' && (
                            <>
                                {(ticket.status === 'OPEN' || ticket.status === 'CLOSURE_REQUESTED') && (
                                    <button
                                        type="button"
                                        disabled={closureLoading}
                                        onClick={handleCloseTicketAction}
                                        className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-red-700 bg-red-50 hover:bg-red-100 rounded-lg transition-colors disabled:opacity-50"
                                    >
                                        <XCircle size={16} />
                                        Zatvori tiket
                                    </button>
                                )}
                                {ticket.status === 'CLOSURE_REQUESTED' && (
                                    <>
                                        <button
                                            type="button"
                                            disabled={closureLoading}
                                            onClick={handleAcceptClosure}
                                            className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-green-700 bg-green-50 hover:bg-green-100 rounded-lg transition-colors disabled:opacity-50"
                                        >
                                            <CheckCircle size={16} />
                                            Prihvati zatvaranje
                                        </button>
                                        <button
                                            type="button"
                                            disabled={closureLoading}
                                            onClick={handleRejectClosure}
                                            className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-amber-700 bg-amber-50 hover:bg-amber-100 rounded-lg transition-colors disabled:opacity-50"
                                        >
                                            <XCircle size={16} />
                                            Odbij zatvaranje
                                        </button>
                                    </>
                                )}
                            </>
                        )}

                        {/* Akcije za Staff */}
                        {(user?.role === 'AGENT' || user?.role === 'TECHNICIAN' || user?.role === 'ADMINISTRATOR') && (
                            <>
                                {ticket.status === 'OPEN' && (
                                    <button
                                        type="button"
                                        disabled={closureLoading}
                                        onClick={handleRequestClosure}
                                        className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-navy-700 bg-navy-50 hover:bg-navy-100 rounded-lg transition-colors disabled:opacity-50"
                                    >
                                        <Clock size={16} />
                                        Zatraži zatvaranje
                                    </button>
                                )}
                                {ticket.status === 'CLOSURE_REQUESTED' && (user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR' || user?.role === 'TECHNICIAN') && (
                                    <div className="flex items-center gap-3">
                                        <button
                                            type="button"
                                            disabled={closureLoading}
                                            onClick={handleForceClose}
                                            className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-red-700 bg-red-50 hover:bg-red-100 rounded-lg transition-colors disabled:opacity-50"
                                            title="Moguće tek nakon 7 dana bez odgovora klijenta"
                                        >
                                            <Zap size={16} />
                                            Prisilno zatvori
                                        </button>
                                        {timeLeft && (
                                            <div className="flex items-center gap-1.5 text-xs font-medium text-amber-600 bg-amber-50 px-3 py-2 rounded-lg border border-amber-100">
                                                <Clock size={14} />
                                                <span>{timeLeft}</span>
                                            </div>
                                        )}
                                    </div>
                                )}
                                {user?.role === 'AGENT' && ticket.status === 'OPEN' && (
                                    <button
                                        type="button"
                                        onClick={handleOpenForward}
                                        className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-navy-700 bg-navy-50 hover:bg-navy-100 rounded-lg transition-colors"
                                    >
                                        <ArrowRightLeft size={16} />
                                        Proslijedi tiket
                                    </button>
                                )}
                            </>
                        )}
                    </div>
                )}

                {/* Notifikacije za Closure Workflow */}
                {closureNotification && (
                    <div className={`mt-4 p-3 rounded-lg text-xs font-medium animate-in fade-in slide-in-from-top-1 ${closureNotification.type === 'success' ? 'bg-green-50 text-green-700 border border-green-100' : 'bg-red-50 text-red-700 border border-red-100'
                        }`}>
                        {closureNotification.message}
                    </div>
                )}
            </section>

            {/* Interni prioritet — samo za Staff (Agent/Admin/Technician), ali editabilno samo za Agent/Admin */}
            {(user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR' || user?.role === 'TECHNICIAN') && (
                <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                    <div className="flex items-center gap-2 mb-4">
                        <Zap size={18} className="text-navy-600" />
                        <h3 className="text-sm font-semibold text-gray-900">Interni prioritet</h3>
                    </div>

                    {priorityNotification && (
                        <div className={`mb-4 p-3 rounded-lg text-xs font-medium animate-in fade-in slide-in-from-top-1 ${priorityNotification.type === 'success' ? 'bg-green-50 text-green-700 border border-green-100' : 'bg-red-50 text-red-700 border border-red-100'
                            }`}>
                            {priorityNotification.message}
                        </div>
                    )}

                    <div className="flex flex-col sm:flex-row sm:items-center gap-4">
                        <div className="flex-1">
                            <p className="text-xs text-gray-500 mb-2 uppercase tracking-wide font-medium">Trenutni interni prioritet</p>
                            <div className="flex items-center gap-3">
                                {ticket.internalPriority ? (
                                    <Badge value={ticket.internalPriority} className="text-sm px-4 py-1" />
                                ) : (
                                    <span className="text-sm text-gray-400 italic">Prioritet nije postavljen</span>
                                )}
                            </div>
                        </div>

                        {(user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR') && (
                            <div className="sm:w-64">
                                <p className="text-xs text-gray-500 mb-2 uppercase tracking-wide font-medium">Promijeni prioritet</p>
                                <select
                                    disabled={updatingPriority}
                                    value={INTERNAL_PRIORITIES.find(p => p.key === ticket.internalPriority)?.value || ''}
                                    onChange={(e) => handleUpdateInternalPriority(Number(e.target.value))}
                                    className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500 disabled:opacity-50"
                                >
                                    <option value="" disabled>Odaberi prioritet...</option>
                                    {INTERNAL_PRIORITIES.map((p) => (
                                        <option key={p.value} value={p.value}>
                                            {p.label}
                                        </option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </div>
                </section>
            )}

            {/* Historija komunikacije — US-15 */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100">
                <div className="px-6 py-4 border-b border-gray-100 flex items-center gap-2">
                    <MessageCircle size={18} className="text-gray-500" />
                    <h3 className="text-sm font-semibold text-gray-900">
                        Razgovor ({allComments.length})
                    </h3>
                </div>

                <div className="p-6 space-y-5">
                    {allComments.length === 0 ? (
                        <p className="text-sm text-gray-400 text-center py-4">
                            Nema poruka u razgovoru.
                        </p>
                    ) : (
                        allComments.map((comment) => {
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
                                    disabled={!message.trim() || isSending}
                                    className="inline-flex items-center gap-2 px-4 py-2 bg-navy-700 hover:bg-navy-800 disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium rounded-lg transition-colors"
                                >
                                    <Send size={16} />
                                    {isSending ? 'Slanje...' : 'Pošalji'}
                                </button>
                            </div>
                        </div>
                    )}
                </div>
            </section>

            {/* US-55, US-56: Modalni prozor za prosljeđivanje tiketa */}
            <Modal
                isOpen={forwardModalOpen}
                onClose={handleCloseForward}
                title={forwardModalTitle}
                size="md"
            >
                {forwardStep === 'choice' && (
                    <div className="space-y-4">
                        <p className="text-sm text-gray-500">
                            Odaberite način prosljeđivanja tiketa drugom agentu.
                        </p>

                        {forwardError && (
                            <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">
                                {forwardError}
                            </p>
                        )}

                        <div className="space-y-3">
                            <p className="text-xs font-medium text-gray-400 uppercase tracking-wide">
                                Proslijedi agentu
                            </p>

                            <button
                                onClick={handleAutoForward}
                                disabled={forwardLoading}
                                className="w-full flex items-center gap-4 p-4 border-2 border-navy-200 hover:border-navy-500 hover:bg-navy-50 rounded-xl text-left transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <div className="w-10 h-10 bg-navy-100 rounded-lg flex items-center justify-center flex-shrink-0">
                                    <Zap size={20} className="text-navy-700" />
                                </div>
                                <div>
                                    <p className="text-sm font-semibold text-gray-900">
                                        Proslijedi najboljem agentu
                                    </p>
                                    <p className="text-xs text-gray-500 mt-0.5">
                                        Sistem automatski bira agenta s najvišim score-om
                                    </p>
                                </div>
                            </button>

                            <button
                                onClick={handleShowAgents}
                                disabled={forwardLoading}
                                className="w-full flex items-center gap-4 p-4 border-2 border-gray-200 hover:border-navy-500 hover:bg-navy-50 rounded-xl text-left transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <div className="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center flex-shrink-0">
                                    <Users size={20} className="text-gray-600" />
                                </div>
                                <div>
                                    <p className="text-sm font-semibold text-gray-900">
                                        Odaberi agenta
                                    </p>
                                    <p className="text-xs text-gray-500 mt-0.5">
                                        Pregledaj listu agenata i odaberi najprikladnijeg
                                    </p>
                                </div>
                            </button>

                            <div className="flex items-center gap-3 pt-1">
                                <div className="flex-1 h-px bg-gray-200" />
                                <span className="text-xs text-gray-400">ili</span>
                                <div className="flex-1 h-px bg-gray-200" />
                            </div>

                            <p className="text-xs font-medium text-gray-400 uppercase tracking-wide">
                                Proslijedi tehničaru
                            </p>

                            <button
                                onClick={handleForwardToTechnician}
                                disabled={forwardLoading}
                                className="w-full flex items-center gap-4 p-4 border-2 border-gray-200 hover:border-navy-500 hover:bg-navy-50 rounded-xl text-left transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <div className="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center flex-shrink-0">
                                    <Wrench size={20} className="text-gray-600" />
                                </div>
                                <div className="flex-1">
                                    <p className="text-sm font-semibold text-gray-900">
                                        Proslijedi tehničaru
                                    </p>
                                    <p className="text-xs text-gray-500 mt-0.5">
                                        Sistem automatski bira tehničara na lokaciji klijenta
                                    </p>
                                </div>
                            </button>
                        </div>

                        {forwardLoading && (
                            <div className="flex justify-center pt-2">
                                <div className="w-6 h-6 border-2 border-navy-600 border-t-transparent rounded-full animate-spin" />
                            </div>
                        )}
                    </div>
                )}

                {forwardStep === 'agents' && (
                    <div className="space-y-4">
                        <p className="text-sm text-gray-500">
                            Lista je sortirana po kompatibilnosti s ovim tiketom.
                        </p>

                        {forwardError && (
                            <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">
                                {forwardError}
                            </p>
                        )}

                        {agentScores.length === 0 ? (
                            <p className="text-sm text-gray-400 text-center py-6">
                                Nema dostupnih agenata za prosljeđivanje.
                            </p>
                        ) : (
                            <div className="space-y-2 max-h-72 overflow-y-auto pr-1">
                                {agentScores.map((agent) => {
                                    const initials = agent.fullName
                                        .split(' ')
                                        .map((p) => p[0])
                                        .join('')
                                        .slice(0, 2)
                                        .toUpperCase()
                                    const isSelected = selectedAgent?.userId === agent.userId

                                    return (
                                        <button
                                            key={agent.userId}
                                            onClick={() => setSelectedAgent(agent)}
                                            className={`w-full p-3 border-2 rounded-xl text-left transition-colors ${isSelected
                                                ? 'border-navy-500 bg-navy-50'
                                                : 'border-gray-200 hover:border-gray-300'
                                                }`}
                                        >
                                            <div className="flex items-center gap-3">
                                                <div className="w-9 h-9 rounded-full bg-navy-100 text-navy-700 flex items-center justify-center text-xs font-semibold flex-shrink-0">
                                                    {initials}
                                                </div>

                                                <div className="flex-1 min-w-0">
                                                    <p className="text-sm font-semibold text-gray-900">
                                                        {agent.fullName}
                                                    </p>
                                                    <p className="text-xs text-gray-500 mt-0.5">
                                                        {agent.resolvedInCategory} riješenih &middot;&nbsp;
                                                        {agent.avgRating > 0 ? agent.avgRating.toFixed(1) : 'N/A'} ocjena &middot;&nbsp;
                                                        {agent.openTickets} aktivnih
                                                    </p>
                                                </div>

                                                <div className="text-right flex-shrink-0">
                                                    <span className="text-sm font-bold text-navy-700">
                                                        {agent.scorePercent}%
                                                    </span>
                                                    <div className="w-16 h-1.5 bg-gray-200 rounded-full mt-1">
                                                        <div
                                                            className="h-1.5 bg-navy-600 rounded-full"
                                                            style={{ width: `${agent.scorePercent}%` }}
                                                        />
                                                    </div>
                                                </div>
                                            </div>
                                        </button>
                                    )
                                })}
                            </div>
                        )}

                        <div className="flex items-center justify-between pt-2 border-t border-gray-100">
                            <button
                                onClick={() => {
                                    setForwardStep('choice')
                                    setSelectedAgent(null)
                                    setForwardError(null)
                                }}
                                className="text-sm text-gray-500 hover:text-gray-700 transition-colors"
                            >
                                ← Nazad
                            </button>
                            <button
                                onClick={handleForwardToAgent}
                                disabled={!selectedAgent || forwardLoading}
                                className="px-4 py-2 bg-navy-700 hover:bg-navy-800 disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium rounded-lg transition-colors"
                            >
                                {forwardLoading ? 'Slanje...' : 'Proslijedi'}
                            </button>
                        </div>
                    </div>
                )}



                {forwardStep === 'success' && (
                    <div className="text-center space-y-4 py-4">
                        <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto">
                            <CheckCircle size={32} className="text-green-600" />
                        </div>
                        <div>
                            <p className="text-base font-semibold text-gray-900">
                                Tiket je uspješno proslijeđen
                            </p>
                            {forwardedTo && (
                                <p className="text-sm text-gray-500 mt-1">
                                    Dodijeljen:{' '}
                                    <strong className="text-gray-700">{forwardedTo.fullName}</strong>
                                    {' '}({forwardedTo.scorePercent}% kompatibilnosti)
                                </p>
                            )}
                        </div>
                        <button
                            onClick={handleCloseForward}
                            className="px-5 py-2 bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium rounded-lg transition-colors"
                        >
                            Zatvori
                        </button>
                    </div>
                )}
            </Modal>
        </div>
    )
}
