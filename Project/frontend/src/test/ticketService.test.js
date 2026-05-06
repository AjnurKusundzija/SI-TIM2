import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
  },
}))

import api from '../services/api'
import { getMyTickets, createTicket } from '../services/ticketService'

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
})