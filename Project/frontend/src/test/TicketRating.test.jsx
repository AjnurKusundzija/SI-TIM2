import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getTicketById: vi.fn(),
  getTicketComments: vi.fn(),
  getTicketRating: vi.fn(),
  createTicketRating: vi.fn(),
  addComment: vi.fn(),
  closeTicket: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  getTicketById: mocks.getTicketById,
  getTicketComments: mocks.getTicketComments,
  getTicketRating: mocks.getTicketRating,
  createTicketRating: mocks.createTicketRating,
  addComment: mocks.addComment,
  closeTicket: mocks.closeTicket,
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
const AGENT_USER = { role: 'AGENT', firstName: 'Selma' }
const ADMIN_USER = { role: 'ADMINISTRATOR', firstName: 'Admin' }

const OPEN_TICKET = {
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

const CLOSED_TICKET = {
  ticketId: 1,
  title: 'Internet ne radi',
  description: 'Nema veze od jutros.',
  status: 'CLOSED',
  priority: 'HIGH',
  problemCategory: 'INTERNET',
  createdDate: '2026-05-01T10:00:00Z',
  clientName: 'Merjem Omerović',
  assignedAgentName: 'Selma Mujić',
}

const EXISTING_RATING = {
  ratingValue: 4,
  ratingComment: 'Odlična usluga!',
  ratingDate: '2026-05-10T12:00:00Z',
  userId: 5,
}

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

describe('US-61: Ocjenjivanje tiketa', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getTicketComments.mockResolvedValue([])
  })

  // AC: modal se automatski otvara za klijenta koji otvori zatvoren tiket bez ocjene
  it('otvara rating modal automatski za klijenta na zatvorenom tiketu bez ocjene', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(null)

    renderTicketDetail(CLIENT_USER)

    await waitFor(() => {
      expect(screen.getByText('Ocijenite uslugu')).toBeInTheDocument()
    })
  })

  // AC: modal se NE otvara ako ocjena već postoji
  it('ne otvara modal ako je tiket već ocijenjen', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(EXISTING_RATING)

    renderTicketDetail(CLIENT_USER)

    await waitFor(() => {
      expect(screen.queryByText('Ocijenite uslugu')).not.toBeInTheDocument()
    })
  })

  // AC: agent ne vidi opciju za ocjenjivanje (modal se ne otvara)
  it('ne prikazuje modal za ocjenjivanje agentu', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(null)

    renderTicketDetail(AGENT_USER)

    await waitFor(() => {
      expect(screen.queryByText('Ocijenite uslugu')).not.toBeInTheDocument()
    })
  })

  // AC: agent vidi read-only prikaz ocjene na zatvorenom tiketu
  it('prikazuje read-only ocjenu agentu na zatvorenom tiketu', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(EXISTING_RATING)

    renderTicketDetail(AGENT_USER)

    await waitFor(() => {
      expect(screen.getByText('Ocjena klijenta')).toBeInTheDocument()
      expect(screen.getByText(/Odlična usluga!/)).toBeInTheDocument()
    })
  })

  // AC: klijent može zatvoriti modal klikom na "Kasnije"
  it('zatvara modal klikom na Kasnije', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(null)

    renderTicketDetail(CLIENT_USER)

    await waitFor(() => {
      expect(screen.getByText('Ocijenite uslugu')).toBeInTheDocument()
    })

    fireEvent.click(screen.getByText('Kasnije'))

    await waitFor(() => {
      expect(screen.queryByText('Ocijenite uslugu')).not.toBeInTheDocument()
    })
  })

  // AC: dugme "Pošalji ocjenu" je disabled dok zvjezdica nije odabrana
  it('onemogucuje slanje dok nema odabrane zvjezdice', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(null)

    renderTicketDetail(CLIENT_USER)

    await waitFor(() => {
      expect(screen.getByText('Pošalji ocjenu')).toBeDisabled()
    })
  })

  // AC: administrator vidi read-only prikaz ocjene na zatvorenom tiketu
  it('prikazuje read-only ocjenu administratoru na zatvorenom tiketu', async () => {
    mocks.getTicketById.mockResolvedValue(CLOSED_TICKET)
    mocks.getTicketRating.mockResolvedValue(EXISTING_RATING)

    renderTicketDetail(ADMIN_USER)

    await waitFor(() => {
      expect(screen.getByText('Ocjena klijenta')).toBeInTheDocument()
      expect(screen.getByText(/Odlična usluga!/)).toBeInTheDocument()
    })
  })

  // AC: modal se otvara automatski nakon što klijent zatvori tiket i nema prethodne ocjene
  it('otvara modal automatski nakon što klijent zatvori tiket', async () => {
    mocks.getTicketById
      .mockResolvedValueOnce(OPEN_TICKET)
      .mockResolvedValueOnce(CLOSED_TICKET)
    mocks.closeTicket.mockResolvedValue(undefined)
    mocks.getTicketRating.mockResolvedValue(null)

    renderTicketDetail(CLIENT_USER)

    await waitFor(() => {
      expect(screen.getByText('Zatvori tiket')).toBeInTheDocument()
    })

    fireEvent.click(screen.getByText('Zatvori tiket'))
    fireEvent.click(screen.getAllByRole('button', { name: 'Zatvori tiket' }).at(-1))

    await waitFor(() => {
      expect(screen.getByText('Ocijenite uslugu')).toBeInTheDocument()
    })
  })
})
