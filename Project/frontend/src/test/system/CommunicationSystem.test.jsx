import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
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

describe('PB-27 Communication system flow — slanje poruke na otvorenom tiketu', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('korisnik otvara tiket, unosi poruku i salje je — addComment se poziva s ispravnim podacima', async () => {
    mocks.getTicketById.mockResolvedValueOnce({
      ticketId: 1,
      title: 'Internet ne radi',
      description: 'Opis.',
      status: 'OPEN',
      priority: 'HIGH',
      problemCategory: 'INTERNET',
      createdDate: '2026-05-01T10:00:00Z',
      clientName: 'Merjem Omerovic',
      assignedAgentName: null,
    })
    mocks.getTicketComments.mockResolvedValueOnce([])
    mocks.addComment.mockResolvedValueOnce({})

    render(
      <MemoryRouter initialEntries={['/tickets/1']}>
        <Routes>
          <Route path="/tickets/:id" element={<TicketDetail />} />
        </Routes>
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.getByPlaceholderText(/unesite vašu poruku/i)).toBeInTheDocument()
    )

    fireEvent.change(screen.getByPlaceholderText(/unesite vašu poruku/i), {
      target: { value: 'Problem i dalje postoji.' },
    })
    fireEvent.click(screen.getByText(/pošalji/i))

    await waitFor(() => {
      expect(mocks.addComment).toHaveBeenCalledWith('1', 'Problem i dalje postoji.')
    })

    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })
})
