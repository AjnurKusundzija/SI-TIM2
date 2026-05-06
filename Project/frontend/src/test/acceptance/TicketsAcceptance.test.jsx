import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAllTickets: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../../services/ticketService', () => ({
  getAllTickets: mocks.getAllTickets,
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('../../components/common/EmptyState', () => ({
  default: ({ title }) => <div>{title}</div>,
}))

import Tickets from '../../pages/Tickets'

describe('PB-32 Tickets acceptance — agent poslovni tok', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'AGENT', firstName: 'Selma' } })
  })

  it('agent vidi sve tikete u sistemu i može pristupiti tiketima koji nisu iz njegove primarne oblasti', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([
      {
        ticketId: 10,
        title: 'Pogrešan iznos na računu',
        status: 'OPEN',
        priority: 'HIGH',
        problemCategory: 'BILLING',
        createdDate: '2026-05-01T10:00:00Z',
      },
      {
        ticketId: 11,
        title: 'Mobilna mreža bez signala',
        status: 'OPEN',
        priority: 'MEDIUM',
        problemCategory: 'MOBILE_NETWORK',
        createdDate: '2026-05-02T09:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <Tickets />
      </MemoryRouter>
    )

    // AC: agent vidi tikete iz svih kategorija (BILLING, MOBILE_NETWORK, itd.)
    await waitFor(() =>
      expect(screen.queryAllByText('Pogrešan iznos na računu')).not.toHaveLength(0)
    )
    expect(screen.queryAllByText('Mobilna mreža bez signala')).not.toHaveLength(0)

    // AC: agent vidi Svi tiketi / Dodijeljeni meni toggle (PB-32)
    expect(screen.getByText('Svi tiketi')).toBeInTheDocument()
    expect(screen.getByText('Dodijeljeni meni')).toBeInTheDocument()

    // AC: nema poruke o grešci — lista se učitava uspješno
    expect(screen.queryByText(/failed to load/i)).not.toBeInTheDocument()
  })
})
