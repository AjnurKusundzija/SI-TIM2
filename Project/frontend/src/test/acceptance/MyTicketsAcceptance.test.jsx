import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getMyTickets: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../../services/ticketService', () => ({
  getMyTickets: mocks.getMyTickets,
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

import MyTickets from '../../pages/MyTickets'

describe('PB-23 MyTickets acceptance — klijent pregledava vlastite tikete', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  it('klijent vidi samo vlastite tikete s ispravnim statusima i moze razlikovati otvorene od zatvorenih', async () => {
    mocks.getMyTickets.mockResolvedValueOnce([
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
        title: 'TV kanal nestaje',
        status: 'CLOSED',
        priority: 'LOW',
        problemCategory: 'TV',
        createdDate: '2026-04-20T09:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <MyTickets />
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    )

    // AC US-11: korisnik vidi vlastite tikete
    expect(screen.queryAllByText('TV kanal nestaje')).not.toHaveLength(0)

    // AC US-12: statusi su prikazani razumljivim jezikom
    expect(screen.queryAllByText('Otvoren')).not.toHaveLength(0)
    expect(screen.queryAllByText('Zatvoren')).not.toHaveLength(0)

    // AC: nema poruke o gresci
    expect(screen.queryByText(/failed|greska|error/i)).not.toBeInTheDocument()
  })
})
