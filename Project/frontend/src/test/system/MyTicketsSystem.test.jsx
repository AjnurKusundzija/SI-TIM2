import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
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

describe('PB-23 MyTickets system flow — pregled i pretraga vlastitih tiketa', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('korisnik otvara listu tiketa, vidi sve statuse i moze pretraziti po naslovu', async () => {
    mocks.getMyTickets.mockResolvedValueOnce([
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
        status: 'CLOSED',
        priority: 'MEDIUM',
        problemCategory: 'TV',
        createdDate: '2026-04-11T09:00:00Z',
      },
    ])

    render(
      <MemoryRouter>
        <MyTickets />
      </MemoryRouter>
    )

    // Korisnik vidi oba tiketa
    await waitFor(() =>
      expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    )
    expect(screen.queryAllByText('TV signal nestaje')).not.toHaveLength(0)

    // OPEN prikazan kao "Otvoren", CLOSED kao "Zatvoren"
    expect(screen.queryAllByText('Otvoren')).not.toHaveLength(0)
    expect(screen.queryAllByText('Zatvoren')).not.toHaveLength(0)

    // Korisnik pretrazuje
    fireEvent.change(screen.getByPlaceholderText(/pretraži|pretrazi/i), {
      target: { value: 'internet' },
    })

    expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    expect(screen.queryAllByText('TV signal nestaje')).toHaveLength(0)

    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })
})
