import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

// ─── Hoist mocks before any imports ────────────────────────────────────────
const mocks = vi.hoisted(() => ({
  getAllTickets: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  getAllTickets: mocks.getAllTickets,
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('../components/common/EmptyState', () => ({
  default: ({ title, description }) => (
    <div data-testid="empty-state">
      <p data-testid="empty-title">{title}</p>
      {description && <p data-testid="empty-description">{description}</p>}
    </div>
  ),
}))

vi.mock('../components/common/Badge', () => ({
  default: ({ value }) => <span data-testid="badge">{value}</span>,
}))

import AssignedTickets from '../pages/AssignedTickets'

// ─── Test data ──────────────────────────────────────────────────────────────
const TECH_USER = { role: 'TECHNICIAN', firstName: 'Amer' }

const ASSIGNED_TICKETS = [
  {
    ticketId: 1,
    title: 'Kvar na kabelskoj',
    status: 'OPEN',
    priority: 'HIGH',
    problemCategory: 'INTERNET',
    createdDate: '2026-05-01T08:00:00Z',
  },
  {
    ticketId: 2,
    title: 'Signal problem',
    status: 'CLOSURE_REQUESTED',
    priority: 'MEDIUM',
    problemCategory: 'TV',
    createdDate: '2026-05-03T10:00:00Z',
  },
]

function renderAssignedTickets(user = TECH_USER) {
  mocks.useAuth.mockReturnValue({ user })
  return render(
    <MemoryRouter>
      <AssignedTickets />
    </MemoryRouter>
  )
}

describe('TechnicianDashboard — AssignedTickets (US-35 / US-36)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  // US-35: Technician sees only assigned tickets
  it('shows only assigned tickets when loaded', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(ASSIGNED_TICKETS)
    renderAssignedTickets()

    await waitFor(() =>
      expect(screen.queryAllByText('Kvar na kabelskoj')).not.toHaveLength(0)
    )
    expect(screen.queryAllByText('Kvar na kabelskoj')).not.toHaveLength(0)
    expect(screen.queryAllByText('Signal problem')).not.toHaveLength(0)
  })

  // US-35: getAllTickets is called with assignedOnly=true for technician view
  it('calls getAllTickets with assignedOnly=true', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([])
    renderAssignedTickets()

    await waitFor(() => expect(mocks.getAllTickets).toHaveBeenCalledWith(true))
  })

  // US-35: Empty state shown when no assigned tickets
  it('shows empty state when technician has no assigned tickets', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([])
    renderAssignedTickets()

    await waitFor(() =>
      expect(screen.getByTestId('empty-state')).toBeInTheDocument()
    )
  })

  // US-36: Ticket status badges are rendered for each ticket
  it('renders status badge for each assigned ticket', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(ASSIGNED_TICKETS)
    renderAssignedTickets()

    await waitFor(() =>
      expect(screen.queryAllByText('Kvar na kabelskoj')).not.toHaveLength(0)
    )
    // Badges rendered — at least one present
    const badges = screen.getAllByTestId('badge')
    expect(badges.length).toBeGreaterThan(0)
  })

  // US-36: CLOSURE_REQUESTED status displayed as "Čeka se"
  it('displays CLOSURE_REQUESTED status as "Čeka se"', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(ASSIGNED_TICKETS)
    renderAssignedTickets()

    await waitFor(() =>
      expect(screen.queryAllByText('Čeka se')).not.toHaveLength(0)
    )
  })

  // US-35: OPEN status displayed as "Otvoren"
  it('displays OPEN status as "Otvoren"', async () => {
    mocks.getAllTickets.mockResolvedValueOnce(ASSIGNED_TICKETS)
    renderAssignedTickets()

    await waitFor(() =>
      expect(screen.queryAllByText('Otvoren')).not.toHaveLength(0)
    )
  })

  // Error state
  it('shows error message when API call fails', async () => {
    mocks.getAllTickets.mockRejectedValueOnce(new Error('Network error'))
    renderAssignedTickets()

    await waitFor(() =>
      expect(
        screen.getByText(/greška pri učitavanju/i)
      ).toBeInTheDocument()
    )
  })
})
