import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getTicketById: vi.fn(),
  getTicketComments: vi.fn(),
  addComment: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../../services/ticketService', () => ({
  getTicketById: mocks.getTicketById,
  getTicketComments: mocks.getTicketComments,
  addComment: mocks.addComment,
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('@microsoft/signalr', () => {
  const connection = {
    start: vi.fn().mockResolvedValue(undefined),
    invoke: vi.fn().mockResolvedValue(undefined),
    on: vi.fn(),
    stop: vi.fn().mockResolvedValue(undefined),
    state: 'Disconnected',
  }
  return {
    HubConnectionBuilder: vi.fn(() => ({
      withUrl: vi.fn().mockReturnThis(),
      withAutomaticReconnect: vi.fn().mockReturnThis(),
      build: vi.fn(() => connection),
    })),
    HubConnectionState: { Connected: 'Connected' },
  }
})

vi.mock('../../components/common/Badge', () => ({
  default: ({ value }) => <span>{value}</span>,
}))

vi.mock('../../components/common/EmptyState', () => ({
  default: ({ title }) => <div>{title}</div>,
}))

import TicketDetail from '../../pages/TicketDetail'

describe('PB-27 Communication acceptance — tok komunikacije klijent-agent', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  it('klijent moze poslati poruku agentu na otvorenom tiketu', async () => {
    mocks.getTicketById.mockResolvedValueOnce({
      ticketId: 5,
      title: 'Mobilna mreza bez signala',
      description: 'Bez signala vec 2 dana.',
      status: 'OPEN',
      priority: 'MEDIUM',
      problemCategory: 'MOBILE_NETWORK',
      createdDate: '2026-05-01T10:00:00Z',
      clientName: 'Merjem Omerovic',
      assignedAgentName: 'Selma Mujic',
    })
    mocks.getTicketComments.mockResolvedValueOnce([])
    mocks.addComment.mockResolvedValueOnce({})

    render(
      <MemoryRouter initialEntries={['/tickets/5']}>
        <Routes>
          <Route path="/tickets/:id" element={<TicketDetail />} />
        </Routes>
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.getByPlaceholderText(/unesite vašu poruku/i)).toBeInTheDocument()
    )

    // AC US-20: input polje je dostupno za otvoreni tiket
    const input = screen.getByPlaceholderText(/unesite vašu poruku/i)
    expect(input).toBeInTheDocument()

    // AC US-20: dugme Pošalji je onemoguceno za prazan unos
    const sendBtn = screen.getByText(/pošalji/i).closest('button')
    expect(sendBtn).toBeDisabled()

    // Korisnik unosi poruku
    fireEvent.change(input, { target: { value: 'Da li je problem riješen?' } })
    expect(sendBtn).not.toBeDisabled()

    fireEvent.click(sendBtn)

    await waitFor(() => {
      // AC US-20: poruka poslana s ispravnim ID-em tiketa
      expect(mocks.addComment).toHaveBeenCalledWith('5', 'Da li je problem riješen?')
    })
  })

  it('input za poruku je skriven kada je tiket zatvoren — komunikacija nije moguca', async () => {
    mocks.getTicketById.mockResolvedValueOnce({
      ticketId: 6,
      title: 'Riješen problem',
      description: 'Opis.',
      status: 'CLOSED',
      priority: 'LOW',
      problemCategory: 'INTERNET',
      createdDate: '2026-04-01T10:00:00Z',
      clientName: 'Merjem Omerovic',
      assignedAgentName: null,
    })
    mocks.getTicketComments.mockResolvedValueOnce([])

    render(
      <MemoryRouter initialEntries={['/tickets/6']}>
        <Routes>
          <Route path="/tickets/:id" element={<TicketDetail />} />
        </Routes>
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.queryAllByText('Riješen problem')).not.toHaveLength(0)
    )

    // AC: slanje poruka nije moguce na zatvorenom tiketu
    expect(screen.queryByPlaceholderText(/unesite vašu poruku/i)).not.toBeInTheDocument()
  })
})
