import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getTicketById: vi.fn(),
  getTicketComments: vi.fn(),
  addComment: vi.fn(),
  getTicketRating: vi.fn(),
  useAuth: vi.fn(),
  HubConnectionBuilder: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  getTicketById: mocks.getTicketById,
  getTicketComments: mocks.getTicketComments,
  addComment: mocks.addComment,
  getTicketRating: mocks.getTicketRating,
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
