import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
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

describe('PB-33 SearchFilter acceptance — agent pretrazuje i filtrira tikete', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'AGENT', firstName: 'Selma' } })
  })

  it('agent moze suziti listu tiketa kombinacijom pretrage i filtera po statusu', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([
      {
        ticketId: 1,
        title: 'Internet ne radi',
        status: 'OPEN',
        priority: 'HIGH',
        problemCategory: 'INTERNET',
        createdDate: '2026-05-01T10:00:00Z',
      },
      {
        ticketId: 2,
        title: 'TV signal nestaje',
        status: 'CLOSED',
        priority: 'LOW',
        problemCategory: 'TV',
        createdDate: '2026-05-02T09:00:00Z',
      },
      {
        ticketId: 3,
        title: 'Internet spor',
        status: 'OPEN',
        priority: 'MEDIUM',
        problemCategory: 'INTERNET',
        createdDate: '2026-05-03T08:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <Tickets />
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    )

    // AC US-31: filter po statusu prikazuje samo otvorene tikete
    fireEvent.change(screen.getByDisplayValue('Svi statusi'), {
      target: { value: 'OPEN' },
    })

    expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    expect(screen.queryAllByText('Internet spor')).not.toHaveLength(0)
    expect(screen.queryAllByText('TV signal nestaje')).toHaveLength(0)

    // Resetuj filter
    fireEvent.change(screen.getByDisplayValue('Otvoren'), {
      target: { value: 'ALL' },
    })

    // AC US-32: pretraga po naslovu suzuje listu
    fireEvent.change(screen.getByPlaceholderText(/pretraži tikete/i), {
      target: { value: 'spor' },
    })

    expect(screen.queryAllByText('Internet spor')).not.toHaveLength(0)
    expect(screen.queryAllByText('Internet ne radi')).toHaveLength(0)
    expect(screen.queryAllByText('TV signal nestaje')).toHaveLength(0)
  })
})
