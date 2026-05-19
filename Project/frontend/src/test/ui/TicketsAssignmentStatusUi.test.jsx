import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAllTickets: vi.fn(),
  useAuth: vi.fn(),
  useNavigate: vi.fn(() => vi.fn()),
}))

vi.mock('../../services/ticketService', () => ({
  getAllTickets: mocks.getAllTickets,
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: mocks.useNavigate,
  }
})

import Tickets from '../../pages/Tickets'

describe('Tickets UI - assignment status display', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'AGENT', firstName: 'Agent' } })
  })

  it('renders status badges for tickets in the assigned ticket list', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([
      {
        ticketId: 1,
        title: 'Dodijeljeni otvoreni tiket',
        status: 'OPEN',
        priority: 'HIGH',
        internalPriority: 'CRITICAL',
        problemCategory: 'INTERNET',
        createdDate: '2026-05-10T08:00:00Z',
      },
      {
        ticketId: 2,
        title: 'Tiket ceka zatvaranje',
        status: 'CLOSURE_REQUESTED',
        priority: 'LOW',
        problemCategory: 'TV',
        createdDate: '2026-05-11T08:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <Tickets assignedOnly />
      </MemoryRouter>
    )

    await waitFor(() => expect(screen.queryAllByText('Dodijeljeni otvoreni tiket')).not.toHaveLength(0))

    expect(screen.queryAllByText('Tiket ceka zatvaranje')).not.toHaveLength(0)
    expect(screen.queryAllByText('OTVOREN')).not.toHaveLength(0)
    expect(screen.queryAllByText('ČEKA SE')).not.toHaveLength(0)
    expect(screen.queryAllByText('KRITICAN')).not.toHaveLength(0)
    expect(mocks.getAllTickets).toHaveBeenCalledWith(true)
  })
})
