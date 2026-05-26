import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getTicketById: vi.fn(),
  getTicketComments: vi.fn(),
  addComment: vi.fn(),
  getTicketRating: vi.fn(),
  selfAssignTicket: vi.fn(),
  useAuth: vi.fn(),
  HubConnectionBuilder: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  getTicketById: mocks.getTicketById,
  getTicketComments: mocks.getTicketComments,
  addComment: mocks.addComment,
  getTicketRating: mocks.getTicketRating,
  selfAssignTicket: mocks.selfAssignTicket,
  createTicketRating: vi.fn(),
  closeTicket: vi.fn(),
  requestTicketClosure: vi.fn(),
  acceptTicketClosure: vi.fn(),
  rejectTicketClosure: vi.fn(),
  forceCloseTicket: vi.fn(),
  autoForwardTicket: vi.fn(),
  forwardTicketToAgent: vi.fn(),
  getAgentScores: vi.fn(),
  forwardTicketToTechnician: vi.fn(),
  updateInternalPriority: vi.fn(),
  updateTicketStatus: vi.fn(),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

// Stub SignalR
vi.mock('@microsoft/signalr', () => {
  const connection = {
    start: vi.fn().mockResolvedValue(undefined),
    invoke: vi.fn().mockResolvedValue(undefined),
    on: vi.fn(),
    stop: vi.fn().mockResolvedValue(undefined),
    state: 'Disconnected',
  }
  return {
    HubConnectionBuilder: vi.fn(() => ({
      withUrl: vi.fn().mockReturnThis(),
      withAutomaticReconnect: vi.fn().mockReturnThis(),
      build: vi.fn(() => connection),
    })),
    HubConnectionState: { Connected: 'Connected' },
  }
})

vi.mock('../components/common/Badge', () => ({
  default: ({ value }) => <span data-testid="badge">{value}</span>,
}))

vi.mock('../components/common/EmptyState', () => ({
  default: ({ title }) => <div data-testid="empty-state">{title}</div>,
}))

import TicketDetail from '../pages/TicketDetail'

const CLIENT_USER = { role: 'CLIENT', firstName: 'Mina' }
const AGENT_USER  = { role: 'AGENT',  firstName: 'Selma' }

const FAKE_TICKET = {
  ticketId: 1,
  title: 'Internet ne radi',
  description: 'Nema veze od jutros.',
  status: 'OPEN',
  priority: 'HIGH',
  problemCategory: 'INTERNET',
  createdDate: '2026-05-01T10:00:00Z',
  clientName: 'Merjem Omerović',
  assignedAgentName: 'Selma Mujić',
  assignedAgentId: 2,
  assignedTechnicianName: 'Adnan Hasić',
  assignedTechnicianId: 7,
}

const FAKE_COMMENTS = [
  {
    commentId: 1,
    content: 'Hvala što ste prijavili problem.',
    authorName: 'Selma Mujić',
    authorRole: 'AGENT',
    dateTime: '2026-05-01T10:30:00Z',
  },
]

function renderTicketDetail(user = CLIENT_USER, ticketId = '1') {
  mocks.useAuth.mockReturnValue({ user })
  return render(
    <MemoryRouter initialEntries={[`/tickets/${ticketId}`]}>
      <Routes>
        <Route path="/tickets/:id" element={<TicketDetail />} />
      </Routes>
    </MemoryRouter>
  )
}

describe('TicketDetail page — PB-24, PB-27', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getTicketRating.mockResolvedValue(null)
  })

  // PB-24 / US-14: detalji tiketa se prikazuju nakon učitavanja
  it('renders ticket details after successful load', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())
    expect(screen.getByText('Internet ne radi')).toBeInTheDocument()
    expect(screen.getByText(/Nema veze od jutros/)).toBeInTheDocument()
  })

  // PB-24 / US-14: prikazuju se ime klijenta i dodjeljenog agenta
  it('shows client name and assigned agent name', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() => expect(screen.queryAllByText('Merjem Omerović')).not.toHaveLength(0))
    expect(screen.queryAllByText('Selma Mujić')).not.toHaveLength(0)
  })

  it('shows assigned technician below the assigned agent', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail(AGENT_USER)

    await waitFor(() => expect(screen.queryAllByText('Selma Mujić')).not.toHaveLength(0))
    expect(screen.queryAllByText('Adnan Hasić')).not.toHaveLength(0)
  })

  // PB-24 / US-15: historija komentara se prikazuje
  it('displays comment history when comments exist', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce(FAKE_COMMENTS)
    renderTicketDetail()

    await waitFor(() =>
      expect(screen.getByText('Hvala što ste prijavili problem.')).toBeInTheDocument()
    )
  })

  // PB-27 / US-20: input za poruku prikazuje se za otvoreni tiket
  it('shows message input for open ticket', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.getByPlaceholderText(/unesite vašu poruku/i)).toBeInTheDocument()
    expect(screen.getByText(/pošalji/i)).toBeInTheDocument()
  })

  // PB-27 / US-20: input za poruku je skriven kada je tiket zatvoren
  it('hides message input for closed ticket', async () => {
    const closedTicket = { ...FAKE_TICKET, status: 'CLOSED' }
    mocks.getTicketById.mockResolvedValueOnce(closedTicket)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByPlaceholderText(/unesite vašu poruku/i)).not.toBeInTheDocument()
  })

  // PB-24 / US-14: prikazuje empty state kada API vrati grešku
  it('shows error empty state when API call fails', async () => {
    mocks.getTicketById.mockRejectedValueOnce(new Error('Not found'))
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() => {
      expect(screen.getByTestId('empty-state')).toBeInTheDocument()
    })
  })

  // PB-27 / US-20: slanje poruke poziva addComment sa ispravnim parametrima
  it('submits a message by calling addComment', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    mocks.addComment.mockResolvedValueOnce({})
    renderTicketDetail()

    await waitFor(() =>
      expect(screen.getByPlaceholderText(/unesite vašu poruku/i)).toBeInTheDocument()
    )

    fireEvent.change(screen.getByPlaceholderText(/unesite vašu poruku/i), {
      target: { value: 'Testna poruka' },
    })
    fireEvent.click(screen.getByText(/pošalji/i))

    await waitFor(() => {
      expect(mocks.addComment).toHaveBeenCalledWith('1', 'Testna poruka')
    })
  })

  // PB-27 / US-20: dugme Pošalji je onemogućeno kada je poruka prazna
  it('disables send button when message is empty', async () => {
    mocks.getTicketById.mockResolvedValueOnce(FAKE_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail()

    await waitFor(() =>
      expect(screen.getByPlaceholderText(/unesite vašu poruku/i)).toBeInTheDocument()
    )

    const sendButton = screen.getByText(/pošalji/i).closest('button')
    expect(sendButton).toBeDisabled()
  })
})

// PB-62 / US-105: Samodjelovanje tiketa za agente („Preuzmi tiket")
describe('TicketDetail — PB-62 self-assign', () => {
  const UNASSIGNED_OPEN_TICKET = {
    ticketId: 5,
    title: 'Internet ne radi',
    description: 'Opis problema.',
    status: 'OPEN',
    priority: 'HIGH',
    problemCategory: 'INTERNET',
    createdDate: '2026-05-01T10:00:00Z',
    clientName: 'Merjem Omerović',
    assignedAgentName: '',
    assignedAgentId: null,
    assignedTechnicianName: '',
    assignedTechnicianId: null,
  }

  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getTicketRating.mockResolvedValue(null)
  })

  it('shows "Preuzmi tiket" button for AGENT when ticket is open and unassigned', async () => {
    mocks.getTicketById.mockResolvedValueOnce(UNASSIGNED_OPEN_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'AGENT', userId: 11 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.getByRole('button', { name: /preuzmi tiket/i })).toBeInTheDocument()
  })

  it('hides "Preuzmi tiket" button for CLIENT', async () => {
    mocks.getTicketById.mockResolvedValueOnce({ ...UNASSIGNED_OPEN_TICKET, creatorId: 1 })
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'CLIENT', userId: 1 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
  })

  it('hides "Preuzmi tiket" button for TECHNICIAN', async () => {
    mocks.getTicketById.mockResolvedValueOnce(UNASSIGNED_OPEN_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'TECHNICIAN', userId: 8 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
  })

  it('hides "Preuzmi tiket" button for ADMINISTRATOR', async () => {
    mocks.getTicketById.mockResolvedValueOnce(UNASSIGNED_OPEN_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'ADMINISTRATOR', userId: 1 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
  })

  it('hides "Preuzmi tiket" button when ticket already has assigned agent', async () => {
    mocks.getTicketById.mockResolvedValueOnce({
      ...UNASSIGNED_OPEN_TICKET,
      assignedAgentId: 99,
      assignedAgentName: 'Druga Osoba',
    })
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'AGENT', userId: 11 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
  })

  it('hides "Preuzmi tiket" button when ticket is closed', async () => {
    mocks.getTicketById.mockResolvedValueOnce({ ...UNASSIGNED_OPEN_TICKET, status: 'CLOSED' })
    mocks.getTicketComments.mockResolvedValueOnce([])
    renderTicketDetail({ role: 'AGENT', userId: 11 }, '5')

    await waitFor(() => expect(screen.getByText('Internet ne radi')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
  })

  it('clicking "Preuzmi tiket" calls selfAssignTicket and refreshes ticket', async () => {
    const assignedTicket = {
      ...UNASSIGNED_OPEN_TICKET,
      assignedAgentId: 11,
      assignedAgentName: 'Selma Mujić',
    }
    mocks.getTicketById
      .mockResolvedValueOnce(UNASSIGNED_OPEN_TICKET)
      .mockResolvedValueOnce(assignedTicket)
    mocks.getTicketComments.mockResolvedValueOnce([])
    mocks.selfAssignTicket.mockResolvedValueOnce({ userId: 11, fullName: 'Selma Mujić' })

    renderTicketDetail({ role: 'AGENT', userId: 11 }, '5')

    await waitFor(() => expect(screen.getByRole('button', { name: /preuzmi tiket/i })).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /preuzmi tiket/i }))

    await waitFor(() => expect(mocks.selfAssignTicket).toHaveBeenCalledWith(5))
    // UI mora pokazati agenta kao dodjeljenog i sakriti dugme
    await waitFor(() =>
      expect(screen.queryByRole('button', { name: /preuzmi tiket/i })).not.toBeInTheDocument()
    )
    expect(screen.getByText('Selma Mujić')).toBeInTheDocument()
  })

  it('shows error message when self-assign fails (backend reject — already assigned)', async () => {
    mocks.getTicketById.mockResolvedValueOnce(UNASSIGNED_OPEN_TICKET)
    mocks.getTicketComments.mockResolvedValueOnce([])
    mocks.selfAssignTicket.mockRejectedValueOnce({
      response: { data: { poruka: 'Tiket je već dodijeljen drugom agentu.' } },
    })

    renderTicketDetail({ role: 'AGENT', userId: 11 }, '5')

    await waitFor(() => expect(screen.getByRole('button', { name: /preuzmi tiket/i })).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /preuzmi tiket/i }))

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent(/već dodijeljen/i)
    )
  })
})
