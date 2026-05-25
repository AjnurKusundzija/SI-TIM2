import { useEffect, useRef, useState } from 'react'
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
    AlertCircle,
    Info,
} from 'lucide-react'
import * as signalR from '@microsoft/signalr'
import {
    addComment,
    addCommentWithAttachments,
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
    getTicketRating,
    createTicketRating,
    updateTicketStatus,
} from '../services/ticketService'
import { useAuth } from '../context/AuthContext'
import Badge from '../components/common/Badge'
import ConfirmDialog from '../components/common/ConfirmDialog'
import EmptyState from '../components/common/EmptyState'
import Modal from '../components/common/Modal'
import AttachmentList from '../components/common/AttachmentList'
import FileUpload from '../components/common/FileUpload'
import { formatDateTime } from '../utils/formatDate'

const MAX_COMMENT_LENGTH = 1000

function TicketDetailSkeleton() {
    return (
        <div className="max-w-5xl mx-auto space-y-5">
            <div className="h-4 w-32 bg-gray-200 rounded animate-pulse" />

            <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-6">
                <div className="flex flex-col lg:flex-row lg:items-start lg:justify-between gap-4">
                    <div className="space-y-2">
                        <div className="h-6 w-64 bg-gray-200 rounded animate-pulse" />
                        <div className="h-3 w-24 bg-gray-100 rounded animate-pulse" />
                    </div>
                    <div className="flex gap-2">
                        <div className="h-6 w-20 bg-gray-100 rounded-full animate-pulse" />
                        <div className="h-6 w-20 bg-gray-100 rounded-full animate-pulse" />
                    </div>
                </div>

                <div className="border-t border-gray-100 pt-4 grid grid-cols-1 md:grid-cols-3 gap-4">
                    {[0, 1, 2].map((item) => (
                        <div key={item} className="h-4 bg-gray-100 rounded animate-pulse" />
                    ))}
                </div>
            </section>

            <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6 space-y-4">
                <div className="h-5 w-36 bg-gray-200 rounded animate-pulse" />
                {[0, 1, 2].map((item) => (
                    <div key={item} className="flex gap-3">
                        <div className="w-9 h-9 rounded-full bg-gray-100 animate-pulse" />
                        <div className="flex-1 space-y-2">
                            <div className="h-4 w-40 bg-gray-100 rounded animate-pulse" />
                            <div className="h-4 w-full bg-gray-100 rounded animate-pulse" />
                        </div>
                    </div>
                ))}
            </section>
        </div>
    )
}

function RatingScale({ value, onChange, readonly = false }) {
    const labels = ['Odaberite ocjenu', 'MOGLO BI BOLJE', 'ISPOD PROSJEKA', 'NORMALNO', 'DOBRO', 'ODLIČNO']

    const mouthPath = [
        'M 32 63 L 68 63',
        'M 32 66 Q 50 52 68 66',
        'M 32 65 Q 50 57 68 65',
        'M 32 63 L 68 63',
        'M 32 60 Q 50 72 68 60',
        'M 30 58 Q 50 78 70 58',
    ]

    const safeValue = value >= 0 && value <= 5 ? value : 0
    const trackRef = useRef(null)
    const dragging = useRef(false)

    const getValueFromX = (clientX) => {
        const rect = trackRef.current?.getBoundingClientRect()
        if (!rect) return safeValue
        const pad = 12 // matches px-3 on the container
        const ratio = Math.max(0, Math.min(1, (clientX - rect.left - pad) / (rect.width - pad * 2)))
        return Math.round(ratio * 4) + 1
    }

    const handlePointerDown = (e) => {
        if (readonly) return
        dragging.current = true
        e.currentTarget.setPointerCapture(e.pointerId)
        onChange?.(getValueFromX(e.clientX))
    }

    const handlePointerMove = (e) => {
        if (!dragging.current || readonly) return
        onChange?.(getValueFromX(e.clientX))
    }

    const handlePointerUp = () => { dragging.current = false }

    return (
        <div className="flex flex-col items-center gap-4 w-full py-2">
            <p className="text-xs font-bold tracking-widest text-navy-700 uppercase min-h-[18px]">
                {labels[safeValue]}
            </p>

            <svg
                viewBox="0 0 100 100"
                className="w-32 h-32 text-navy-700"
                fill="none"
                stroke="currentColor"
                strokeWidth="4.5"
                strokeLinecap="round"
            >
                <circle cx="50" cy="50" r="44" />
                <circle cx="35" cy="38" r="4" fill="currentColor" stroke="none" />
                <circle cx="65" cy="38" r="4" fill="currentColor" stroke="none" />
                <path d={mouthPath[safeValue]} />
            </svg>

            <div
                ref={trackRef}
                className={`w-full relative select-none mt-1 py-3 px-3 ${readonly ? '' : 'cursor-pointer'}`}
                onPointerDown={handlePointerDown}
                onPointerMove={handlePointerMove}
                onPointerUp={handlePointerUp}
                onPointerLeave={handlePointerUp}
            >
                <div className="absolute left-3 right-3 h-[3px] bg-gray-200 rounded-full top-[21px]" />

                <div className="relative h-[24px]">
                    {[1, 2, 3, 4, 5].map((n) => {
                        const isSelected = n === safeValue
                        return (
                            <div
                                key={n}
                                style={{ left: `${(n - 1) * 25}%` }}
                                className={`absolute top-1/2 -translate-y-1/2 -translate-x-1/2 rounded-full transition-all duration-150 z-10 pointer-events-none ${
                                    isSelected
                                        ? 'w-6 h-6 bg-navy-700 shadow-md ring-2 ring-navy-200'
                                        : 'w-3 h-3 bg-gray-300'
                                }`}
                            />
                        )
                    })}
                </div>

                <div className="relative h-6 mt-2">
                    {[1, 2, 3, 4, 5].map((n) => (
                        <span
                            key={n}
                            style={{ left: `${(n - 1) * 25}%` }}
                            className={`absolute -translate-x-1/2 text-sm font-semibold transition-colors pointer-events-none ${
                                n === safeValue ? 'text-navy-700' : 'text-gray-400'
                            }`}
                        >
                            {n}
                        </span>
                    ))}
                </div>
            </div>
        </div>
    )
}

export default function TicketDetail() {
    const { id } = useParams()
    const navigate = useNavigate()
    const { user } = useAuth()

    const backPath = user?.role === 'CLIENT' ? '/mytickets' : '/tickets'

    const [ticket, setTicket] = useState(null)
    const [comments, setComments] = useState([])
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState(null)

    const [message, setMessage] = useState('')
    const [files, setFiles] = useState([])
    const [showFileUpload, setShowFileUpload] = useState(false)
    const [isSending, setIsSending] = useState(false)
    const [sendError, setSendError] = useState(null)

    // Forward modal state
    const [forwardModalOpen, setForwardModalOpen] = useState(false)
    const [forwardStep, setForwardStep] = useState('choice') // 'choice' | 'agents' | 'success'
    const [agentScores, setAgentScores] = useState([])
    const [selectedAgent, setSelectedAgent] = useState(null)
    const [forwardLoading, setForwardLoading] = useState(false)
    const [forwardError, setForwardError] = useState(null)
    const [forwardedTo, setForwardedTo] = useState(null)

    // Closure workflow state
    const [closureNotification, setClosureNotification] = useState(null)
    const [timeLeft, setTimeLeft] = useState('')
    const [timeLeftMs, setTimeLeftMs] = useState(null)

    // Confirm dialog state
    const [confirmCloseOpen, setConfirmCloseOpen] = useState(false)

    // US-61: Rating states
    const [rating, setRating] = useState(undefined) // undefined = not fetched yet, null = no rating
    const [ratingModalDismissed, setRatingModalDismissed] = useState(false)
    const [ratingValue, setRatingValue] = useState(0)
    const [ratingComment, setRatingComment] = useState('')
    const [ratingLoading, setRatingLoading] = useState(false)
    const [ratingError, setRatingError] = useState(null)
    const [ratingSuccess, setRatingSuccess] = useState(false)

    // US-61: Modal je otvoren kada klijent ima zatvoren tiket bez ocjene (izvedena vrijednost, nema useEffect)
    const ratingModalOpen = (
        ticket?.status === 'CLOSED' &&
        user?.role === 'CLIENT' &&
        rating === null &&
        !ratingModalDismissed
    ) || ratingSuccess

    // PB-36 / US-60: Status promjena (tehničar)
    const TECHNICIAN_STATUSES = [
        { value: 'OPEN', label: 'U toku (Otvoren)' },
        { value: 'CLOSURE_REQUESTED', label: 'Čeka se' },
    ]
    const [statusUpdating, setStatusUpdating] = useState(false)
    const [statusNotification, setStatusNotification] = useState(null)

    const handleUpdateTicketStatus = async (newStatus) => {
        if (!newStatus || newStatus === ticket?.status) return
        setStatusUpdating(true)
        setStatusNotification(null)
        try {
            await updateTicketStatus(Number(id), newStatus)
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setStatusNotification({ type: 'success', message: 'Status tiketa je uspješno ažuriran.' })
        } catch (err) {
            console.error('Failed to update ticket status', err)
            setStatusNotification({
                type: 'error',
                message: err.response?.data?.poruka || 'Greška pri ažuriranju statusa tiketa.',
            })
        } finally {
            setStatusUpdating(false)
            setTimeout(() => setStatusNotification(null), 3000)
        }
    }

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

                // US-61: Učitaj ocjenu za zatvorene tikete (CLIENT, AGENT, ADMINISTRATOR)
                if (
                    fetchedTicket.status === 'CLOSED' &&
                    (user?.role === 'CLIENT' || user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR')
                ) {
                    return getTicketRating(ticketId)
                        .then(fetchedRating => { setRating(fetchedRating) })
                        .catch(() => setRating(null))
                }
            })
            .catch((err) => {
                console.error(err)
                setError('Nije moguće učitati detalje tiketa.')
            })
            .finally(() => setLoading(false))
    }, [id, user?.role])

    useEffect(() => {
        const updateTimer = () => {
            if (ticket?.status !== 'CLOSURE_REQUESTED') {
                setTimeLeft('')
                setTimeLeftMs(null)
                return
            }

            const clientComments = comments.filter(c => c.authorRole === 'CLIENT')
            const lastCommentDate = clientComments.length > 0
                ? new Date(Math.max(...clientComments.map(c => new Date(c.dateTime))))
                : new Date(ticket.createdDate)

            const expireDate = new Date(lastCommentDate.getTime() + 7 * 24 * 60 * 60 * 1000)
            const now = new Date()
            const diff = expireDate - now

            setTimeLeftMs(diff)

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
        if (!id) return

        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build()

        newConnection.start()
            .then(() => {
                newConnection.invoke('JoinTicketGroup', id).catch(e => console.error(e))
                newConnection.on('ReceiveComment', (comment) => {
                    setComments((prev) => [...prev, comment])
                })
            })
            .catch(e => console.error('SignalR Connection Error: ', e))

        return () => {
            if (newConnection.state === signalR.HubConnectionState.Connected) {
                newConnection.invoke('LeaveTicketGroup', id).catch(console.error)
            }
            newConnection.stop()
        }
    }, [id])

   // Izmena u handleSend da podrži praznu poruku sa fajlovima
const handleSend = async () => {
    // Dozvoli slanje ako ima poruke ILI fajlova
    if ((!message.trim() && files.length === 0) || isSending) return;
    
    setSendError(null);
    setIsSending(true);
    try {
        if (files.length > 0) {
            // Osiguraj da message nije null/undefined za backend
            await addCommentWithAttachments(id, message || "", files);
        } else {
            await addComment(id, message);
        }
        setMessage('');
        setFiles([]);
        setShowFileUpload(false);
    } catch (err) {
        // ... error handling
    } finally {
        setIsSending(false);
    }
};

// Izmena na samom dugmetu (JSX deo)
<button 
    type="button" 
    onClick={handleSend} 
    disabled={(!message.trim() && files.length === 0) || isSending}
    className="..."
>
    {isSending ? 'Slanje...' : 'Pošalji'}
</button>

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
    const [priorityNotification, setPriorityNotification] = useState(null)

    const handleUpdateInternalPriority = async (newPriorityValue) => {
        setUpdatingPriority(true)
        setPriorityNotification(null)
        try {
            await updateInternalPriority(Number(id), newPriorityValue)
            setPriorityNotification({ type: 'success', message: 'Interni prioritet je uspješno ažuriran.' })
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
        } catch (err) {
            console.error('Failed to update internal priority', err)
            setPriorityNotification({ type: 'error', message: err.response?.data?.message || 'Greška pri ažuriranju prioriteta.' })
        } finally {
            setUpdatingPriority(false)
            setTimeout(() => setPriorityNotification(null), 3000)
        }
    }

    // Closure Workflow Actions
    const [closureLoading, setClosureLoading] = useState(false)

    const handleCloseTicketAction = async () => {
        setClosureLoading(true)
        try {
            await closeTicket(Number(id))
            const updatedTicket = await getTicketById(Number(id))
            setTicket(updatedTicket)
            setClosureNotification({ type: 'success', message: 'Tiket je uspješno zatvoren.' })
            if (user?.role === 'CLIENT') {
                const currentRating = await getTicketRating(Number(id)).catch(() => null)
                setRating(currentRating)
            }
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
            if (user?.role === 'CLIENT') {
                const currentRating = await getTicketRating(Number(id)).catch(() => null)
                setRating(currentRating)
            }
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

    // US-61: Slanje ocjene
    const handleSubmitRating = async () => {
        if (!ratingValue) return
        setRatingLoading(true)
        setRatingError(null)
        try {
            const newRating = await createTicketRating(Number(id), {
                ratingValue,
                ratingComment: ratingComment || '',
            })
            setRating(newRating)
            setRatingSuccess(true)
        } catch (err) {
            setRatingError(err.response?.data?.poruka || 'Greška pri slanju ocjene.')
        } finally {
            setRatingLoading(false)
        }
    }

    const forwardModalTitle = {
        choice: 'Proslijedi tiket',
        agents: 'Odaberi agenta',
        success: 'Tiket proslijeđen',
    }[forwardStep]

    if (loading) return <TicketDetailSkeleton />

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
    const createdDate = formatDateTime(ticket.createdDate)
    const countdownClass = timeLeftMs !== null && timeLeftMs <= 0
        ? 'bg-red-50 text-red-700 border-red-100'
        : timeLeftMs !== null && timeLeftMs <= 86400000
            ? 'bg-amber-50 text-amber-700 border-amber-100'
            : 'bg-gray-50 text-gray-600 border-gray-100'

    const clientName = ticket.clientName || 'Klijent'
    const agentName = ticket.assignedAgentName || 'Nije dodijeljen'
    const technicianName = ticket.assignedTechnicianName || ''
    const isAssignedStaff = user?.role === 'ADMINISTRATOR'
        || ticket.assignedAgentId === user?.userId
        || ticket.assignedTechnicianId === user?.userId

    const initialComment = {
        commentId: 'initial',
        authorName: clientName,
        authorRole: 'CLIENT',
        dateTime: ticket.createdDate,
        content: ticket.description || 'Nema opisa za ovaj tiket.',
        // PB-56 / US-81: Prikaži attachments iz originalnog tiketa
        attachments: ticket.attachments || []
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

            {/* Ticket details */}
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
                    <div className="flex flex-col sm:flex-row sm:items-center gap-2">
                        <div className="flex items-center gap-2">
                            <User size={16} />
                            <span>Kreirao: <strong className="text-gray-700">{clientName}</strong></span>
                        </div>
                        {ticket.creatorId > 0 && (user?.role === 'AGENT' || user?.role === 'TECHNICIAN' || user?.role === 'ADMINISTRATOR') && (
                            <Link
                                to={`/users/${ticket.creatorId}`}
                                className="text-sm text-navy-700 hover:text-navy-900 underline"
                            >
                                Pogledaj profil korisnika
                            </Link>
                        )}
                    </div>
                    <div className="flex flex-col gap-2">
                        <div className="flex items-center gap-2">
                            <Tag size={16} />
                            <span>Agent: <strong className="text-gray-700">{agentName}</strong></span>
                        </div>
                        {technicianName && (
                            <div className="flex items-center gap-2">
                                <Wrench size={16} />
                                <span>Tehničar: <strong className="text-gray-700">{technicianName}</strong></span>
                            </div>
                        )}
                    </div>
                    <div className="flex items-center gap-2">
                        <Clock size={16} />
                        <span>Kreirano: <strong className="text-gray-700">{createdDate}</strong></span>
                    </div>
                </div>

                {ticket.status !== 'CLOSED' && (
                    <div className="border-t border-gray-100 mt-5 pt-4 flex flex-wrap gap-2">
                        {/* Client actions */}
                        {user?.role === 'CLIENT' && (
                            <>
                                {(ticket.status === 'OPEN' || ticket.status === 'CLOSURE_REQUESTED') && (
                                    <button
                                        type="button"
                                        disabled={closureLoading}
                                        onClick={() => setConfirmCloseOpen(true)}
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

                        {/* Staff actions */}
                        {(user?.role === 'AGENT' || user?.role === 'TECHNICIAN' || user?.role === 'ADMINISTRATOR') && (
                            <>
                                {ticket.status === 'OPEN' && (
                                    <button
                                        type="button"
                                        disabled={closureLoading || !isAssignedStaff}
                                        onClick={handleRequestClosure}
                                        title={!isAssignedStaff
                                            ? 'Samo dodijeljeni agent ili tehničar može zatražiti zatvaranje'
                                            : undefined
                                        }
                                        className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-navy-700 bg-navy-50 hover:bg-navy-100 rounded-lg transition-colors disabled:opacity-50"
                                    >
                                        <Clock size={16} />
                                        Zatraži zatvaranje
                                    </button>
                                )}
                                {ticket.status === 'CLOSURE_REQUESTED' && (
                                    <div className="flex items-center gap-3 flex-wrap">
                                        <button
                                            type="button"
                                            disabled={closureLoading || !isAssignedStaff}
                                            onClick={handleForceClose}
                                            className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-red-700 bg-red-50 hover:bg-red-100 rounded-lg transition-colors disabled:opacity-50"
                                            title={!isAssignedStaff
                                                ? 'Samo dodijeljeni agent ili tehničar može prisilno zatvoriti tiket'
                                                : 'Moguće tek nakon 7 dana bez odgovora klijenta'
                                            }
                                        >
                                            <Zap size={16} />
                                            Prisilno zatvori
                                        </button>
                                        {timeLeft && (
                                            <div className={`flex items-center gap-1.5 text-xs font-medium px-3 py-2 rounded-lg border ${countdownClass}`}>
                                                <Clock size={14} />
                                                <span>{timeLeft}</span>
                                                {timeLeftMs !== null && timeLeftMs <= 86400000 && (
                                                    <AlertCircle size={13} className="ml-0.5" />
                                                )}
                                            </div>
                                        )}
                                    </div>
                                )}
                                {user?.role === 'AGENT' && ticket.status === 'OPEN' && (
                                    <button
                                        type="button"
                                        disabled={closureLoading || ticket.assignedAgentId !== user?.userId}
                                        onClick={handleOpenForward}
                                        title={ticket.assignedAgentId !== user?.userId ? 'Samo dodijeljeni agent može proslijediti tiket' : undefined}
                                        className="inline-flex items-center gap-2 px-3 py-2 text-sm font-medium text-navy-700 bg-navy-50 hover:bg-navy-100 rounded-lg transition-colors disabled:opacity-50"
                                    >
                                        <ArrowRightLeft size={16} />
                                        Proslijedi tiket
                                    </button>
                                )}
                            </>
                        )}
                    </div>
                )}

                {/* PB-36 / US-60: Tehničar mijenja status tiketa koji mu je dodijeljen */}
                {user?.role === 'TECHNICIAN'
                    && ticket.status !== 'CLOSED'
                    && ticket.assignedTechnicianId === user?.userId && (
                    <div className="border-t border-gray-100 mt-5 pt-4">
                        <div className="flex flex-col sm:flex-row sm:items-end gap-3">
                            <div className="flex-1">
                                <p className="text-xs text-gray-500 mb-2 uppercase tracking-wide font-medium">
                                    Promijeni status tiketa
                                </p>
                                <select
                                    aria-label="Promijeni status tiketa"
                                    disabled={statusUpdating}
                                    value={ticket.status || ''}
                                    onChange={(e) => handleUpdateTicketStatus(e.target.value)}
                                    className="w-full sm:w-72 px-3 py-2 border border-gray-300 rounded-lg text-sm bg-white outline-none focus:ring-2 focus:ring-navy-500 disabled:opacity-50"
                                >
                                    {TECHNICIAN_STATUSES.map((s) => (
                                        <option key={s.value} value={s.value}>{s.label}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        {statusNotification && (
                            <div
                                role="status"
                                className={`mt-3 p-3 rounded-lg text-xs font-medium ${
                                    statusNotification.type === 'success'
                                        ? 'bg-green-50 text-green-700 border border-green-100'
                                        : 'bg-red-50 text-red-700 border border-red-100'
                                }`}
                            >
                                {statusNotification.message}
                            </div>
                        )}
                    </div>
                )}

                {/* Closure workflow notification */}
                {closureNotification && (
                    <div className={`mt-4 p-3 rounded-lg text-xs font-medium ${closureNotification.type === 'success' ? 'bg-green-50 text-green-700 border border-green-100' : 'bg-red-50 text-red-700 border border-red-100'}`}>
                        {closureNotification.message}
                    </div>
                )}
            </section>

            {/* Internal priority — visible to staff, editable by agent/admin */}
            {(user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR' || user?.role === 'TECHNICIAN') && (
                <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                    <div className="flex items-center gap-2 mb-4">
                        <Zap size={18} className="text-navy-600" />
                        <h3 className="text-sm font-semibold text-gray-900">Interni prioritet</h3>
                    </div>

                    {priorityNotification && (
                        <div className={`mb-4 p-3 rounded-lg text-xs font-medium ${priorityNotification.type === 'success' ? 'bg-green-50 text-green-700 border border-green-100' : 'bg-red-50 text-red-700 border border-red-100'}`}>
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
                                        <option key={p.value} value={p.value}>{p.label}</option>
                                    ))}
                                </select>
                            </div>
                        )}
                    </div>
                </section>
            )}

            {/* Conversation history */}
            <section className="bg-white rounded-xl shadow-sm border border-gray-100">
                <div className="px-6 py-4 border-b border-gray-100 flex items-center gap-2">
                    <MessageCircle size={18} className="text-gray-500" />
                    <h3 className="text-sm font-semibold text-gray-900">
                        Razgovor ({allComments.length})
                    </h3>
                </div>

                <div className="p-6 space-y-5">
                    {allComments.map((comment) => {
                        if (comment.isSystemMessage) {
                            return (
                                <div key={comment.commentId} className="flex items-center gap-3 py-1">
                                    <div className="flex-1 border-t border-gray-100" />
                                    <span className="text-xs text-gray-400 bg-gray-50 px-3 py-1 rounded-full border border-gray-100 whitespace-nowrap flex items-center gap-1.5">
                                        <Info size={11} />
                                        {comment.content}
                                    </span>
                                    <div className="flex-1 border-t border-gray-100" />
                                </div>
                            )
                        }

                        const nameParts = comment.authorName.split(' ')
                        const initials = (
                            nameParts.length >= 2
                                ? `${nameParts[0][0]}${nameParts[nameParts.length - 1][0]}`
                                : comment.authorName.slice(0, 2)
                        ).toUpperCase()

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
                                            {formatDateTime(comment.dateTime)}
                                        </span>
                                    </div>

                                    <p className="text-sm text-gray-600 mt-1 leading-6">
                                        {comment.content}
                                    </p>

                                    {/* PB-56 / US-81: Prikazi attachments ako postoje */}
                                    {comment.attachments && comment.attachments.length > 0 && (
                                        <AttachmentList attachments={comment.attachments} />
                                    )}
                                </div>
                            </div>
                        )
                    })}

                    {/* Message input */}
                    {ticket.status !== 'CLOSED' && (
                        <div className="space-y-2 border-t border-gray-100 pt-4">
                            <textarea
                                value={message}
                                onChange={(e) => {
                                    if (e.target.value.length <= MAX_COMMENT_LENGTH) {
                                        setMessage(e.target.value)
                                        if (sendError) setSendError(null)
                                    }
                                }}
                                onKeyDown={(e) => {
                                    if (e.key === 'Enter' && (e.ctrlKey || e.metaKey)) handleSend()
                                }}
                                rows={3}
                                placeholder="Unesite vašu poruku... (Ctrl+Enter za slanje)"
                                className="w-full px-3 py-3 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none"
                            />

                            {sendError && (
                                <div className="flex items-center gap-2 text-xs text-red-600 bg-red-50 border border-red-100 rounded-lg px-3 py-2">
                                    <AlertCircle size={13} />
                                    {sendError}
                                </div>
                            )}

                            {/* PB-56 / US-80: Upload attachment-ima uz poruku */}
                            {files.length > 0 && (
                                <div>
                                    <p className="text-xs font-medium text-gray-600 mb-2">Odabrani fajlovi:</p>
                                    <FileUpload onFilesSelected={setFiles} maxFiles={5} compact={true} />
                                </div>
                            )}

                            {files.length === 0 && !showFileUpload && (
                                <button
                                    type="button"
                                    onClick={() => setShowFileUpload(true)}
                                    className="text-xs text-navy-600 hover:text-navy-700 font-medium"
                                >
                                    + Dodaj prilog
                                </button>
                            )}

                            {showFileUpload && files.length === 0 && (
                                <div>
                                    <FileUpload onFilesSelected={(selectedFiles) => {
                                        setFiles(selectedFiles)
                                        if (selectedFiles.length > 0) {
                                            setShowFileUpload(false)
                                        }
                                    }} maxFiles={5} compact={true} />
                                </div>
                            )}

                            <div className="flex items-center justify-between">
                                <span className={`text-xs ${message.length >= MAX_COMMENT_LENGTH ? 'text-red-500 font-medium' : 'text-gray-400'}`}>
                                    {message.length} / {MAX_COMMENT_LENGTH}
                                </span>
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

            {/* US-61: Poziv na ocjenjivanje za CLIENT (bez ocjene) */}
            {ticket.status === 'CLOSED' && user?.role === 'CLIENT' && rating === null && (
                <section className="bg-navy-50 rounded-xl border border-navy-100 p-5 flex items-center justify-between gap-4">
                    <div>
                        <p className="text-sm font-semibold text-navy-800">Ocijenite pruženu uslugu</p>
                        <p className="text-xs text-navy-600 mt-0.5">Vaše mišljenje nam pomaže da unaprijedimo kvalitet usluge.</p>
                    </div>
                    <button
                        type="button"
                        onClick={() => setRatingModalDismissed(false)}
                        className="flex-shrink-0 px-4 py-2 bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium rounded-lg transition-colors"
                    >
                        Ocijeni uslugu
                    </button>
                </section>
            )}

            {/* US-61: Read-only prikaz ocjene za Agent i Administrator */}
            {ticket.status === 'CLOSED' &&
                (user?.role === 'AGENT' || user?.role === 'ADMINISTRATOR') &&
                rating && (
                    <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
                        <div className="flex items-center gap-2 mb-3">
                            <span className="text-sm font-semibold text-gray-900">Ocjena klijenta</span>
                        </div>
                        <RatingScale value={rating.ratingValue} readonly />
                        {rating.ratingComment && (
                            <p className="mt-3 text-sm text-gray-600 italic text-center">
                                &ldquo;{rating.ratingComment}&rdquo;
                            </p>
                        )}
                        <p className="mt-1 text-xs text-gray-400 text-center">
                            {new Date(rating.ratingDate).toLocaleString('bs-BA')}
                        </p>
                    </section>
                )}

            {/* US-61: Modal za ocjenjivanje tiketa (CLIENT) */}
            <Modal
                isOpen={ratingModalOpen}
                onClose={() => { setRatingModalDismissed(true); setRatingSuccess(false) }}
                title={ratingSuccess ? 'Hvala!' : 'Ocijenite uslugu'}
                size="sm"
            >
                {ratingSuccess ? (
                    <div className="text-center space-y-5 py-4">
                        <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto">
                            <CheckCircle size={32} className="text-green-600" />
                        </div>
                        <div className="space-y-1">
                            <p className="text-base font-semibold text-gray-900">Hvala na ocjeni!</p>
                            <p className="text-sm text-gray-500">
                                Cijenimo vaše mišljenje i koristit ćemo ga za poboljšanje kvaliteta usluge.
                            </p>
                        </div>
                        <button
                            type="button"
                            onClick={() => { setRatingModalDismissed(true); setRatingSuccess(false) }}
                            className="px-5 py-2 bg-navy-700 hover:bg-navy-800 text-white text-sm font-medium rounded-lg transition-colors"
                        >
                            Zatvori
                        </button>
                    </div>
                ) : (
                    <div className="space-y-4">
                        <p className="text-sm text-gray-500 text-center">
                            Vaš tiket je zatvoren. Ocijenite kvalitet pružene usluge. Vaše mišljenje nam je bitno.
                        </p>

                        <RatingScale value={ratingValue} onChange={setRatingValue} />

                        <div>
                            <p className="text-xs font-medium text-gray-500 mb-2 uppercase tracking-wide">
                                Komentar (opcionalno)
                            </p>
                            <textarea
                                value={ratingComment}
                                onChange={(e) => setRatingComment(e.target.value)}
                                rows={3}
                                placeholder="Podijelite vaše iskustvo..."
                                className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:ring-2 focus:ring-navy-500 focus:border-navy-500 outline-none resize-none"
                            />
                        </div>

                        {ratingError && (
                            <p className="text-sm text-red-600 bg-red-50 px-3 py-2 rounded-lg">
                                {ratingError}
                            </p>
                        )}

                        <div className="flex items-center justify-between pt-2 border-t border-gray-100">
                            <button
                                type="button"
                                onClick={() => setRatingModalDismissed(true)}
                                className="text-sm text-gray-500 hover:text-gray-700 transition-colors"
                            >
                                Kasnije
                            </button>
                            <button
                                type="button"
                                onClick={handleSubmitRating}
                                disabled={!ratingValue || ratingLoading}
                                className="px-4 py-2 bg-navy-700 hover:bg-navy-800 disabled:opacity-40 disabled:cursor-not-allowed text-white text-sm font-medium rounded-lg transition-colors"
                            >
                                {ratingLoading ? 'Slanje...' : 'Pošalji ocjenu'}
                            </button>
                        </div>
                    </div>
                )}
            </Modal>

            {/* US-55, US-56: Modalni prozor za prosljeđivanje tiketa */}
            <Modal
                isOpen={forwardModalOpen}
                onClose={handleCloseForward}
                title={forwardModalTitle}
                size="md"
            >
                {/* Step indicator — shown for choice and agents steps */}
                {(forwardStep === 'choice' || forwardStep === 'agents') && (
                    <div className="flex items-center gap-1.5 justify-center mb-5">
                        {['choice', 'agents'].map((step, i) => (
                            <div key={step} className="flex items-center gap-1.5">
                                <div className={`w-6 h-6 rounded-full text-[11px] font-bold flex items-center justify-center ${forwardStep === step ? 'bg-navy-600 text-white' : 'bg-gray-100 text-gray-400'}`}>
                                    {i + 1}
                                </div>
                                {i < 1 && <div className={`w-8 h-0.5 ${forwardStep === 'agents' ? 'bg-navy-400' : 'bg-gray-200'}`} />}
                            </div>
                        ))}
                    </div>
                )}

                {forwardStep === 'choice' && (
                    <div className="space-y-4">
                        <p className="text-sm text-gray-500">
                            Odaberite način prosljeđivanja tiketa.
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
                                        Sistem automatski bira agenta s najvišim score-om za ovu kategoriju
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
                                        Odaberi agenta ručno
                                    </p>
                                    <p className="text-xs text-gray-500 mt-0.5">
                                        Pregledaj listu agenata sortiranu po kompatibilnosti
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
                        <div className="flex items-start gap-2 p-3 bg-blue-50 border border-blue-100 rounded-lg">
                            <Info size={14} className="text-blue-500 flex-shrink-0 mt-0.5" />
                            <p className="text-xs text-blue-700">
                                Score prikazuje kompatibilnost agenta s kategorijom i historijom tiketa. Viši score = bolji match.
                            </p>
                        </div>

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
                                    const nameParts = agent.fullName.split(' ')
                                    const initials = (
                                        nameParts.length >= 2
                                            ? `${nameParts[0][0]}${nameParts[nameParts.length - 1][0]}`
                                            : agent.fullName.slice(0, 2)
                                    ).toUpperCase()
                                    const isSelected = selectedAgent?.userId === agent.userId

                                    return (
                                        <button
                                            key={agent.userId}
                                            onClick={() => setSelectedAgent(agent)}
                                            className={`w-full p-3 border-2 rounded-xl text-left transition-colors ${isSelected ? 'border-navy-500 bg-navy-50' : 'border-gray-200 hover:border-gray-300'}`}
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
                                                    <p className="text-[10px] text-gray-400 leading-none mt-0.5">poklapanje</p>
                                                    <div className="w-16 h-1.5 bg-gray-200 rounded-full mt-1.5">
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
                                    {' '}({forwardedTo.scorePercent}% poklapanje)
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

            {/* Confirm close ticket dialog — replaces window.confirm */}
            <ConfirmDialog
                isOpen={confirmCloseOpen}
                onClose={() => setConfirmCloseOpen(false)}
                onConfirm={handleCloseTicketAction}
                title="Zatvori tiket"
                message="Jeste li sigurni da želite zatvoriti ovaj tiket? Ova akcija se ne može poništiti."
                confirmText="Zatvori tiket"
                cancelText="Odustani"
                variant="danger"
            />
        </div>
    )
}
