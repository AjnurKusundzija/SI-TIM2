import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter, Route, Routes } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getTicketById: vi.fn(),
  getTicketComments: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../../services/ticketService', () => ({
  getTicketById: mocks.getTicketById,
  getTicketComments: mocks.getTicketComments,
  addComment: vi.fn(),
  getTicketRating: vi.fn().mockResolvedValue(null),
  createTicketRating: vi.fn(),
  closeTicket: vi.fn(),
  requestTicketClosure: vi.fn(),
  acceptTicketClosure: vi.fn(),
  rejectTicketClosure: vi.fn(),
  forceCloseTicket: vi.fn(),
  autoForwardTicket: vi.fn(),
  forwardTicketToAgent: vi.fn(),
  getAgentScores: vi.fn(),
  forwardTicketToTechnician: vi.fn(),
  updateInternalPriority: vi.fn(),
  updateTicketStatus: vi.fn(),
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

describe('PB-24 TicketDetail acceptance — klijent pregledava detalje tiketa', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT', firstName: 'Merjem' } })
  })

  it('klijent vidi kompletan prikaz tiketa s opisom, imenom agenta i historijom komunikacije', async () => {
    mocks.getTicketById.mockResolvedValueOnce({
      ticketId: 1,
      title: 'Internet ne radi',
      description: 'Nema veze od jutros.',
      status: 'OPEN',
      priority: 'HIGH',
      problemCategory: 'INTERNET',
      createdDate: '2026-05-01T10:00:00Z',
      clientName: 'Merjem Omerovic',
      assignedAgentName: 'Selma Mujic',
    })
    mocks.getTicketComments.mockResolvedValueOnce([
      {
        commentId: 1,
        content: 'Radimo na problemu, uskoro cemo ga rijesiti.',
        authorName: 'Selma Mujic',
        authorRole: 'AGENT',
        dateTime: '2026-05-01T11:00:00Z',
      },
    ])

    render(
      <MemoryRouter initialEntries={['/tickets/1']}>
        <Routes>
          <Route path="/tickets/:id" element={<TicketDetail />} />
        </Routes>
      </MemoryRouter>
    )

    await waitFor(() =>
      expect(screen.queryAllByText('Internet ne radi')).not.toHaveLength(0)
    )

    // AC US-14: naslov i opis tiketa su vidljivi
    expect(screen.queryAllByText(/Nema veze od jutros/)).not.toHaveLength(0)

    // AC US-14: ime dodijeljenog agenta je vidljivo
    expect(screen.queryAllByText('Selma Mujic')).not.toHaveLength(0)

    // AC US-15: historija komentara je vidljiva
    expect(screen.queryAllByText('Radimo na problemu, uskoro cemo ga rijesiti.')).not.toHaveLength(0)

    // AC: nema poruke o gresci
    expect(screen.queryByText(/failed|greska|error/i)).not.toBeInTheDocument()
  })
})
