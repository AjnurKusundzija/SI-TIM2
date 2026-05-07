import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
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

describe('PB-22 CreateTicket system flow — tok kreiranja tiketa', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('korisnik popunjava formu i uspjesno kreira tiket — forma se resetuje', async () => {
    const navigate = vi.fn()
    mocks.useNavigate.mockReturnValue(navigate)
    mocks.createTicket.mockResolvedValueOnce({ ticketId: 1, title: 'Internet ne radi' })

    render(
      <MemoryRouter>
        <CreateTicket />
      </MemoryRouter>
    )

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
      expect(mocks.createTicket).toHaveBeenCalledWith({
        Subject: 'Internet ne radi',
        Type: 'INTERNET',
        Description: 'Nema veze od jutros.',
        Priority: 'HIGH',
      })
    })

    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })

  it('forma odbija submit bez naslova — createTicket se ne poziva', async () => {
    render(
      <MemoryRouter>
        <CreateTicket />
      </MemoryRouter>
    )

    fireEvent.submit(screen.getByRole('button', { name: /kreiraj tiket/i }).closest('form'))

    expect(mocks.createTicket).not.toHaveBeenCalled()
  })
})
