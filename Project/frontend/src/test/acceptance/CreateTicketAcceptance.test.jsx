import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  createTicket: vi.fn(),
  useAuth: vi.fn(),
  useNavigate: vi.fn(() => vi.fn()),
}))

vi.mock('../../services/ticketService', () => ({
  createTicket: mocks.createTicket,
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('react-router-dom', async (importOriginal) => {
  const actual = await importOriginal()
  return { ...actual, useNavigate: mocks.useNavigate }
})

import CreateTicket from '../../pages/CreateTicket'

describe('PB-22 CreateTicket acceptance — klijent prijavljuje kvar', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
    mocks.useNavigate.mockReturnValue(vi.fn())
  })

  it('klijent popunjava formu za prijavljivanje kvara i tiket se uspjesno kreira', async () => {
    mocks.createTicket.mockResolvedValueOnce({ ticketId: 42, title: 'Internet ne radi' })

    render(
      <MemoryRouter>
        <CreateTicket />
      </MemoryRouter>
    )

    // AC US-8: forma ima sva potrebna polja
    expect(screen.getByLabelText(/naslov/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/tip problema/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/opis/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/prioritet/i)).toBeInTheDocument()

    fireEvent.change(screen.getByLabelText(/naslov/i), {
      target: { value: 'Internet ne radi' },
    })
    fireEvent.change(screen.getByLabelText(/tip problema/i), {
      target: { value: 'INTERNET' },
    })
    fireEvent.change(screen.getByLabelText(/opis/i), {
      target: { value: 'Nema veze od jutros.' },
    })
    fireEvent.change(screen.getByLabelText(/prioritet/i), {
      target: { value: 'HIGH' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /kreiraj tiket/i }).closest('form'))

    await waitFor(() => {
      // AC US-8: createTicket pozvan s ispravnim podacima
      expect(mocks.createTicket).toHaveBeenCalledWith(
        expect.objectContaining({
          Subject: 'Internet ne radi',
          Type: 'INTERNET',
          Description: 'Nema veze od jutros.',
          Priority: 'HIGH',
        })
      )
    })
  })
})
