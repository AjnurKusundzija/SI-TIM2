import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
  },
}))

import api from '../services/api'
import { getMyTickets, createTicket, getAllTickets, getTicketById, getTicketComments, addComment, selfAssignTicket } from '../services/ticketService'

describe('ticketService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  // ─── getMyTickets (US-11) ─────────────────────────────────────────────────

  // US-11: dohvat tiketa poziva ispravan endpoint
  it('getMyTickets() calls GET /mytickets', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getMyTickets()

    expect(api.get).toHaveBeenCalledWith('/mytickets')
  })

  // US-11: vraća podatke iz API odgovora
  it('getMyTickets() returns data from API response', async () => {
    const tickets = [{ ticketId: 1, title: 'Internet problem', status: 'OPEN' }]
    api.get.mockResolvedValueOnce({ data: tickets })

    const result = await getMyTickets()

    expect(result).toEqual(tickets)
  })

  // US-11/US-13: vraća praznu listu kada korisnik nema tiketa
  it('getMyTickets() returns empty array when user has no tickets', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    const result = await getMyTickets()

    expect(result).toEqual([])
  })

  // ─── createTicket (US-8) ──────────────────────────────────────────────────

  // US-8: kreiranje tiketa poziva ispravan endpoint sa podacima
  it('createTicket() calls POST /tickets with ticket data', async () => {
    const ticketData = { Subject: 'Problem', Type: 'INTERNET', Description: 'Opis', Priority: 'HIGH' }
    api.post.mockResolvedValueOnce({ data: { ticketId: 1 } })

    await createTicket(ticketData)

    expect(api.post).toHaveBeenCalledWith('/tickets', ticketData)
  })

  // US-8: vraća kreiran tiket iz API odgovora
  it('createTicket() returns created ticket from API response', async () => {
    const created = { ticketId: 5, title: 'Problem', status: 'OPEN' }
    api.post.mockResolvedValueOnce({ data: created })

    const result = await createTicket({ Subject: 'Problem', Type: 'INTERNET', Description: 'D', Priority: 'LOW' })

    expect(result).toEqual(created)
  })

  // ─── getAllTickets (PB-32) ────────────────────────────────────────────────

  it('getAllTickets() calls GET /tickets without params by default', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getAllTickets()

    expect(api.get).toHaveBeenCalledWith('/tickets', { params: {} })
  })

  it('getAllTickets(true) calls GET /tickets with assignedOnly param', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getAllTickets(true)

    expect(api.get).toHaveBeenCalledWith('/tickets', { params: { assignedOnly: true } })
  })

  it('getAllTickets() returns array of tickets', async () => {
    const tickets = [{ ticketId: 1 }, { ticketId: 2 }]
    api.get.mockResolvedValueOnce({ data: tickets })

    const result = await getAllTickets()

    expect(result).toEqual(tickets)
  })

  // ─── getTicketById (US-14, US-30) ────────────────────────────────────────

  it('getTicketById() calls GET /tickets/:id', async () => {
    api.get.mockResolvedValueOnce({ data: { ticketId: 7 } })

    await getTicketById(7)

    expect(api.get).toHaveBeenCalledWith('/tickets/7')
  })

  it('getTicketById() returns ticket data', async () => {
    const ticket = { ticketId: 7, status: 'OPEN' }
    api.get.mockResolvedValueOnce({ data: ticket })

    const result = await getTicketById(7)

    expect(result).toEqual(ticket)
  })

  // ─── getTicketComments (US-15) ────────────────────────────────────────────

  it('getTicketComments() calls GET /comment/tickets/:id', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getTicketComments(3)

    expect(api.get).toHaveBeenCalledWith('/comment/tickets/3')
  })

  it('getTicketComments() returns comments array', async () => {
    const comments = [{ commentId: 1, content: 'Hello' }]
    api.get.mockResolvedValueOnce({ data: comments })

    const result = await getTicketComments(3)

    expect(result).toEqual(comments)
  })

  // ─── addComment (PB-27) ──────────────────────────────────────────────────

  it('addComment() calls POST /comment/tickets/:id with content', async () => {
    api.post.mockResolvedValueOnce({ data: { commentId: 9 } })

    await addComment(5, 'Nova poruka')

    expect(api.post).toHaveBeenCalledWith('/comment/tickets/5', { content: 'Nova poruka' })
  })

  it('addComment() returns created comment', async () => {
    const comment = { commentId: 9, content: 'Nova poruka' }
    api.post.mockResolvedValueOnce({ data: comment })

    const result = await addComment(5, 'Nova poruka')

    expect(result).toEqual(comment)
  })

  // ─── selfAssignTicket (PB-62 / US-105) ───────────────────────────────────

  it('selfAssignTicket() calls POST /tickets/:id/self-assign', async () => {
    api.post.mockResolvedValueOnce({ data: { userId: 10, fullName: 'Agent Test' } })

    await selfAssignTicket(42)

    expect(api.post).toHaveBeenCalledWith('/tickets/42/self-assign')
  })

  it('selfAssignTicket() returns assignment response from API', async () => {
    const response = { userId: 10, fullName: 'Agent Test', scorePercent: 100 }
    api.post.mockResolvedValueOnce({ data: response })

    const result = await selfAssignTicket(42)

    expect(result).toEqual(response)
  })
})