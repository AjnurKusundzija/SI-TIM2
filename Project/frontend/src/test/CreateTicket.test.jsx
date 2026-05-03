import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  createTicket: vi.fn(),
}))

vi.mock('../services/ticketService', () => ({
  createTicket: mocks.createTicket,
}))

import CreateTicket from '../pages/CreateTicket'

describe('CreateTicket page', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  function fillForm({
    subject = 'Internet problem',
    type = 'INTERNET',
    description = 'Ne radi internet',
    priority = 'HIGH',
  } = {}) {
    if (subject !== undefined)
      fireEvent.change(screen.getByLabelText(/naslov/i), { target: { value: subject } })
    if (type !== undefined)
      fireEvent.change(screen.getByLabelText(/tip problema/i), { target: { value: type } })
    if (description !== undefined)
      fireEvent.change(screen.getByLabelText(/opis/i), { target: { value: description } })
    if (priority !== undefined)
      fireEvent.change(screen.getByLabelText(/prioritet/i), { target: { value: priority } })
  }

  function submit() {
    fireEvent.submit(screen.getByRole('button', { name: /kreiraj tiket/i }).closest('form'))
  }

  // ─── Prikaz forme (US-8, US-9, US-10) ────────────────────────────────────

  // US-8: forma prikazuje sva obavezna polja
  it('renders all required form fields', () => {
    render(<CreateTicket />)

    expect(screen.getByLabelText(/naslov/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/tip problema/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/opis/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/prioritet/i)).toBeInTheDocument()
  })

  // US-9: dropdown za tip problema prikazuje svih 5 kategorija
  it('renders all 5 problem categories as options', () => {
    render(<CreateTicket />)

    expect(screen.getByRole('option', { name: 'Internet' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'TV' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'Mobilna mreža' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'Računi/Naplata' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'Tehnička podrška' })).toBeInTheDocument()
  })

  // US-9: dropdown za prioritet prikazuje sva 3 nivoa
  it('renders all 3 priority options', () => {
    render(<CreateTicket />)

    expect(screen.getByRole('option', { name: 'Nizak' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'Srednji' })).toBeInTheDocument()
    expect(screen.getByRole('option', { name: 'Visok' })).toBeInTheDocument()
  })

  // ─── Validacija obaveznih polja (US-8, US-9, US-10) ──────────────────────

  // US-8: submit bez naslova prikazuje grešku i ne poziva servis
  it('shows validation error when subject is empty', async () => {
    render(<CreateTicket />)
    fillForm({ subject: '' })
    submit()

    await waitFor(() => {
      expect(screen.getByText('Naslov je obavezan.')).toBeInTheDocument()
    })
    expect(mocks.createTicket).not.toHaveBeenCalled()
  })

  // US-10: submit bez opisa prikazuje grešku i ne poziva servis
  it('shows validation error when description is empty', async () => {
    render(<CreateTicket />)
    fillForm({ description: '' })
    submit()

    await waitFor(() => {
      expect(screen.getByText('Opis je obavezan.')).toBeInTheDocument()
    })
    expect(mocks.createTicket).not.toHaveBeenCalled()
  })

  // US-9: submit bez tipa problema prikazuje grešku i ne poziva servis
  it('shows validation error when problem type is not selected', async () => {
    render(<CreateTicket />)
    fillForm({ type: '' })
    submit()

    await waitFor(() => {
      expect(screen.getByText('Tip problema je obavezan.')).toBeInTheDocument()
    })
    expect(mocks.createTicket).not.toHaveBeenCalled()
  })

  // US-9: submit bez prioriteta prikazuje grešku i ne poziva servis
  it('shows validation error when priority is not selected', async () => {
    render(<CreateTicket />)
    fillForm({ priority: '' })
    submit()

    await waitFor(() => {
      expect(screen.getByText('Prioritet je obavezan.')).toBeInTheDocument()
    })
    expect(mocks.createTicket).not.toHaveBeenCalled()
  })

  // ─── Uspješno kreiranje (US-8) ────────────────────────────────────────────

  // US-8: uspješno kreiranje prikazuje poruku potvrde
  it('shows success message after ticket is created', async () => {
    mocks.createTicket.mockResolvedValueOnce({ ticketId: 1 })
    render(<CreateTicket />)

    fillForm()
    submit()

    await waitFor(() => {
      expect(screen.getByText(/tiket je uspješno kreiran/i)).toBeInTheDocument()
    })
  })

  // US-8: forma se čisti nakon uspješnog kreiranja
  it('resets form fields after successful ticket creation', async () => {
    mocks.createTicket.mockResolvedValueOnce({ ticketId: 1 })
    render(<CreateTicket />)

    fillForm()
    submit()

    await waitFor(() => {
      expect(screen.getByLabelText(/naslov/i)).toHaveValue('')
      expect(screen.getByLabelText(/opis/i)).toHaveValue('')
    })
  })

  // US-8: createTicket se poziva sa ispravno formatiranim podacima
  it('calls createTicket with correct payload', async () => {
    mocks.createTicket.mockResolvedValueOnce({ ticketId: 1 })
    render(<CreateTicket />)

    fillForm()
    submit()

    await waitFor(() => {
      expect(mocks.createTicket).toHaveBeenCalledWith({
        Subject: 'Internet problem',
        Type: 'INTERNET',
        Description: 'Ne radi internet',
        Priority: 'HIGH',
      })
    })
  })

  // ─── Greške pri kreiranju (US-8) ──────────────────────────────────────────

  // US-8: 401 greška prikazuje poruku o neovlaštenom pristupu
  it('shows unauthorized error message on 401 response', async () => {
    mocks.createTicket.mockRejectedValueOnce({ response: { status: 401 } })
    render(<CreateTicket />)

    fillForm()
    submit()

    await waitFor(() => {
      expect(screen.getByText(/niste ovlašteni/i)).toBeInTheDocument()
    })
  })

  // US-8: generalna greška prikazuje poruku o neuspjehu
  it('shows generic error message on unexpected API error', async () => {
    mocks.createTicket.mockRejectedValueOnce(new Error('Network error'))
    render(<CreateTicket />)

    fillForm()
    submit()

    await waitFor(() => {
      expect(screen.getByText(/nije uspjelo kreiranje tiketa/i)).toBeInTheDocument()
    })
  })

  // ─── Poništi (US-8) ───────────────────────────────────────────────────────

  // US-8: dugme "Poništi" briše unesene podatke iz forme
  it('clears form when reset button is clicked', () => {
    render(<CreateTicket />)

    fireEvent.change(screen.getByLabelText(/naslov/i), { target: { value: 'Test naslov' } })
    fireEvent.click(screen.getByRole('button', { name: /poništi/i }))

    expect(screen.getByLabelText(/naslov/i)).toHaveValue('')
  })
})