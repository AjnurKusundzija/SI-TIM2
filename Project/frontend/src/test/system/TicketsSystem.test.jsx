import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
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

describe('PB-32 Tickets system flow — agent pregled svih tiketa', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'AGENT', firstName: 'Selma' } })
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('agent otvara listu svih tiketa, vidi tikete od različitih klijenata i može ih pretražiti', async () => {
    mocks.getAllTickets.mockResolvedValueOnce([
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
        title: 'TV signal nestaje',
        status: 'OPEN',
        priority: 'MEDIUM',
        problemCategory: 'TV',
        createdDate: '2026-04-11T09:00:00Z',
      },
      {
        ticketId: 3,
        title: 'Račun netačan',
        status: 'CLOSED',
        priority: 'LOW',
        problemCategory: 'BILLING',
        createdDate: '2026-04-12T10:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <Tickets />
      </MemoryRouter>
    )

    // Agent vidi sve tikete
    await waitFor(() =>
      expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    )
    expect(screen.queryAllByText('TV signal nestaje')).not.toHaveLength(0)
    expect(screen.queryAllByText('Račun netačan')).not.toHaveLength(0)

    // Agent pretražuje tikete po naslovu
    fireEvent.change(screen.getByPlaceholderText(/pretraži tikete/i), {
      target: { value: 'internet' },
    })

    // Nakon pretrage, samo odgovarajući tiket je vidljiv
    expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    expect(screen.queryAllByText('TV signal nestaje')).toHaveLength(0)

    // Nema console.error grešaka
    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })
})
