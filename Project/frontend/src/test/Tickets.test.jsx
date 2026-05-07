import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAllTickets: vi.fn(),
  useAuth: vi.fn(),
  useNavigate: vi.fn(() => vi.fn()),
}))

vi.mock('../services/ticketService', () => ({
  getAllTickets: mocks.getAllTickets,
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('../components/common/EmptyState', () => ({
  default: ({ title, description }) => (
    <div>
      <p data-testid="empty-title">{title}</p>
      <p data-testid="empty-description">{description}</p>
    </div>
  ),
}))

import Tickets from '../pages/Tickets'

const AGENT_USER = { role: 'AGENT', firstName: 'Selma' }
const ADMIN_USER = { role: 'ADMINISTRATOR', firstName: 'Admin' }

const FAKE_TICKETS = [
  {
    ticketId: 1,
    title: 'Internet ne radi',
    status: 'OPEN',
    priority: 'HIGH',
    problemCategory: 'INTERNET',
    createdDate: '2026-04-10T08:00:00Z',
  },
  {
    ticketId: 2,
    title: 'TV problem',
    status: 'CLOSED',
    priority: 'LOW',
    problemCategory: 'TV',
    createdDate: '2026-04-12T10:00:00Z',
  },
]

function renderTickets(user = AGENT_USER) {
  mocks.useAuth.mockReturnValue({ user })
  return render(
    <MemoryRouter>
      <Tickets />
    </MemoryRouter>
  )
}

// U jsdom-u renderuje se i desktop tabela i mobilne kartice, pa tekstovi postoje dvaput
const present = (text) => expect(screen.queryAllByText(text)).not.toHaveLength(0)
const absent  = (text) => expect(screen.queryAllByText(text)).toHaveLength(0)

describe('Tickets page — PB-32, PB-33', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  // PB-32: lista tiketa se prikazuje nakon uspješnog dohvata
  it('renders ticket list when tickets are loaded', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0))
    present('Internet ne radi')
    present('TV problem')
  })

  // PB-32 / US-12: OPEN status prikazuje se kao "Otvoren"
  it('displays OPEN status as "Otvoren"', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([FAKE_TICKETS[0]])
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Otvoren')).not.toHaveLength(0))
    present('Otvoren')
  })

  // PB-32 / US-12: CLOSED status prikazuje se kao "Zatvoren"
  it('displays CLOSED status as "Zatvoren"', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([FAKE_TICKETS[1]])
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Zatvoren')).not.toHaveLength(0))
    present('Zatvoren')
  })

  // PB-33: filter po statusu prikazuje samo odgovarajuće tikete
  it('filters tickets by status', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('TV problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi statusi'), { target: { value: 'CLOSED' } })

    present('TV problem')
    absent('Internet ne radi')
  })

  // PB-33: filter po tipu problema prikazuje samo odgovarajuće tikete
  it('filters tickets by problem type', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi tipovi'), { target: { value: 'INTERNET' } })

    present('Internet ne radi')
    absent('TV problem')
  })

  // PB-33: filter po prioritetu prikazuje samo odgovarajuće tikete
  it('filters tickets by priority', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi prioriteti'), { target: { value: 'HIGH' } })

    present('Internet ne radi')
    absent('TV problem')
  })

  // PB-33: pretraga po naslovu filtrira tikete
  it('filters tickets by search term', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0))

    fireEvent.change(screen.getByPlaceholderText(/pretraži tikete/i), { target: { value: 'tv' } })

    present('TV problem')
    absent('Internet ne radi')
  })

  // PB-32: prazno stanje kada nema tiketa
  it('shows empty state when there are no tickets', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([])
    renderTickets()

    await waitFor(() => {
      expect(screen.getByTestId('empty-title')).toBeInTheDocument()
    })
  })

  // PB-32: greška pri učitavanju prikazuje poruku greške
  it('shows error message when API call fails', async () => {
    mocks.getAllTickets.mockRejectedValueOnce(new Error('Network error'))
    renderTickets()

    await waitFor(() => {
      expect(screen.getByText(/failed to load tickets/i)).toBeInTheDocument()
    })
  })

  // PB-32: agent vidi toggle "Svi tiketi / Dodijeljeni meni"
  it('shows assigned-only toggle for agent role', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderTickets(AGENT_USER)

    await waitFor(() => expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0))

    expect(screen.getByText('Svi tiketi')).toBeInTheDocument()
    expect(screen.getByText('Dodijeljeni meni')).toBeInTheDocument()
  })
})
