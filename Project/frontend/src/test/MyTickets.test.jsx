import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getMyTickets: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  getMyTickets: mocks.getMyTickets,
}))

vi.mock('../components/common/EmptyState', () => ({
  default: ({ title, description }) => (
    <div>
      <p data-testid="empty-title">{title}</p>
      <p data-testid="empty-description">{description}</p>
    </div>
  ),
}))

import MyTickets from '../pages/MyTickets'

const FAKE_TICKETS = [
  {
    ticketId: 1,
    title: 'Internet problem',
    status: 'OPEN',
    priority: 'HIGH',
    problemCategory: 'INTERNET',
    createdDate: '2026-04-01T10:00:00Z',
  },
  {
    ticketId: 2,
    title: 'TV problem',
    status: 'CLOSED',
    priority: 'LOW',
    problemCategory: 'TV',
    createdDate: '2026-04-02T10:00:00Z',
  },
]

function renderMyTickets() {
  return render(
    <MemoryRouter>
      <MyTickets />
    </MemoryRouter>
  )
}

// Komponenta renderuje i desktop tabelu i mobile kartice u jsdomu (CSS media queries
// se ne primjenjuju), pa svaki tekst tiketa postoji dvaput u DOM-u.
// Koristimo queryAllByText za provjeru prisustva/odsustva.
const present = (text) => expect(screen.queryAllByText(text)).not.toHaveLength(0)
const absent  = (text) => expect(screen.queryAllByText(text)).toHaveLength(0)

describe('MyTickets page', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  // ─── Prikaz tiketa (US-11) ────────────────────────────────────────────────

  // US-11: lista prikazuje tikete prijavljenog korisnika
  it('renders ticket list when tickets are loaded', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))
    present('Internet problem')
    present('TV problem')
  })

  // ─── Prikaz statusa (US-12) ───────────────────────────────────────────────

  // US-12: OPEN status se prikazuje kao "Otvoren"
  it('displays OPEN status as "Otvoren"', async () => {
    mocks.getMyTickets.mockResolvedValueOnce([FAKE_TICKETS[0]])
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Otvoren')).not.toHaveLength(0))
    present('Otvoren')
  })

  // US-12: CLOSED status se prikazuje kao "Zatvoren"
  it('displays CLOSED status as "Zatvoren"', async () => {
    mocks.getMyTickets.mockResolvedValueOnce([FAKE_TICKETS[1]])
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Zatvoren')).not.toHaveLength(0))
    present('Zatvoren')
  })

  // ─── Prazno stanje (US-11, US-13) ────────────────────────────────────────

  // US-11: prikazuje "Još nema tiketa" kada korisnik nema tiketa
  it('shows empty state when user has no tickets', async () => {
    mocks.getMyTickets.mockResolvedValueOnce([])
    renderMyTickets()

    await waitFor(() => {
      expect(screen.getByTestId('empty-title')).toHaveTextContent('Još nema tiketa')
    })
  })

  // US-13: prikazuje poruku kada filteri ne daju rezultate
  it('shows "no matching tickets" message when filters yield no results', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi prioriteti'), { target: { value: 'MEDIUM' } })

    await waitFor(() => {
      expect(screen.getByTestId('empty-title')).toHaveTextContent('Nijedan tiket ne odgovara filterima')
    })
  })

  // ─── Filtriranje (US-13) ──────────────────────────────────────────────────

  // US-13: filter po prioritetu prikazuje samo odgovarajuće tikete
  it('filters tickets by priority', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi prioriteti'), { target: { value: 'HIGH' } })

    present('Internet problem')
    absent('TV problem')
  })

  // US-13: filter po statusu prikazuje samo odgovarajuće tikete
  it('filters tickets by status', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('TV problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi statusi'), { target: { value: 'CLOSED' } })

    present('TV problem')
    absent('Internet problem')
  })

  // US-13: filter po tipu problema prikazuje samo odgovarajuće tikete
  it('filters tickets by problem type', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi tipovi'), { target: { value: 'TV' } })

    present('TV problem')
    absent('Internet problem')
  })

  // US-13: pretraga po naslovu filtrira tikete
  it('filters tickets by search term in title', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByPlaceholderText(/pretraži tikete/i), { target: { value: 'internet' } })

    present('Internet problem')
    absent('TV problem')
  })

  // US-13: "Očisti sve" uklanja sve aktivne filtere i vraća sve tikete
  it('clears all filters when "Očisti sve" is clicked', async () => {
    mocks.getMyTickets.mockResolvedValueOnce(FAKE_TICKETS)
    renderMyTickets()

    await waitFor(() => expect(screen.queryAllByText('Internet problem')).not.toHaveLength(0))

    fireEvent.change(screen.getByDisplayValue('Svi prioriteti'), { target: { value: 'HIGH' } })
    absent('TV problem')

    fireEvent.click(screen.getByText('Očisti sve'))

    await waitFor(() => expect(screen.queryAllByText('TV problem')).not.toHaveLength(0))
    present('Internet problem')
    present('TV problem')
  })

  // ─── Greška pri učitavanju (US-11) ────────────────────────────────────────

  // US-11: prikazuje grešku kada API poziv ne uspije
  it('shows error message when API call fails', async () => {
    mocks.getMyTickets.mockRejectedValueOnce(new Error('Network error'))
    renderMyTickets()

    await waitFor(() => {
      expect(screen.getByText(/failed to load tickets/i)).toBeInTheDocument()
    })
  })
})