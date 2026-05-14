import { describe, it, expect, vi, beforeEach } from 'vitest'
import {
  getOpenAssignedTickets,
  getClosedAssignedTickets,
  getAgentScores,
  autoForwardTicket,
  forwardTicketToAgent,
  forwardTicketToTechnician,
  updateInternalPriority,
  closeTicket,
  requestTicketClosure,
  acceptTicketClosure,
  rejectTicketClosure,
  forceCloseTicket
} from '../services/ticketService'
import api from '../services/api'

vi.mock('../services/api')

describe('ticketService - Advanced Functions', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getOpenAssignedTickets', () => {
    it('should fetch open assigned tickets', async () => {
      const mockTickets = [
        { ticketId: 1, title: 'Open Ticket 1', status: 'OPEN' },
        { ticketId: 2, title: 'Open Ticket 2', status: 'OPEN' }
      ]

      api.get.mockResolvedValue({ data: mockTickets })

      const result = await getOpenAssignedTickets()

      expect(api.get).toHaveBeenCalledWith('/tickets/assigned/open')
      expect(result).toEqual(mockTickets)
    })

    it('should throw error when API call fails', async () => {
      api.get.mockRejectedValue(new Error('Network error'))

      await expect(getOpenAssignedTickets()).rejects.toThrow('Network error')
    })
  })

  describe('getClosedAssignedTickets', () => {
    it('should fetch closed assigned tickets', async () => {
      const mockTickets = [
        { ticketId: 3, title: 'Closed Ticket 1', status: 'CLOSED' }
      ]

      api.get.mockResolvedValue({ data: mockTickets })

      const result = await getClosedAssignedTickets()

      expect(api.get).toHaveBeenCalledWith('/tickets/assigned/closed')
      expect(result).toEqual(mockTickets)
    })
  })

  describe('getAgentScores', () => {
    it('should fetch agent scores for ticket forwarding', async () => {
      const ticketId = 10
      const mockScores = [
        { agentId: 1, agentName: 'Agent 1', score: 0.95 },
        { agentId: 2, agentName: 'Agent 2', score: 0.85 }
      ]

      api.get.mockResolvedValue({ data: mockScores })

      const result = await getAgentScores(ticketId)

      expect(api.get).toHaveBeenCalledWith(`/tickets/${ticketId}/forward/agents`)
      expect(result).toEqual(mockScores)
    })
  })

  describe('autoForwardTicket', () => {
    it('should auto-forward ticket to best agent', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Ticket forwarded successfully' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await autoForwardTicket(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/forward/auto`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('forwardTicketToAgent', () => {
    it('should forward ticket to specific agent', async () => {
      const ticketId = 10
      const targetAgentId = 5
      const mockResponse = { message: 'Ticket forwarded to agent 5' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await forwardTicketToAgent(ticketId, targetAgentId)

      expect(api.post).toHaveBeenCalledWith(
        `/tickets/${ticketId}/forward/agent`,
        { targetAgentId }
      )
      expect(result).toEqual(mockResponse)
    })
  })

  describe('forwardTicketToTechnician', () => {
    it('should forward ticket to technician', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Ticket forwarded to technician' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await forwardTicketToTechnician(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/forward/technician`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('updateInternalPriority', () => {
    it('should update internal priority of ticket', async () => {
      const ticketId = 10
      const priority = 'HIGH'
      const mockResponse = { message: 'Priority updated' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await updateInternalPriority(ticketId, priority)

      expect(api.post).toHaveBeenCalledWith(
        `/tickets/${ticketId}/internal-priority`,
        { priority }
      )
      expect(result).toEqual(mockResponse)
    })
  })

  describe('closeTicket', () => {
    it('should close ticket', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Ticket closed' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await closeTicket(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/close`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('requestTicketClosure', () => {
    it('should request ticket closure', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Closure requested' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await requestTicketClosure(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/request-closure`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('acceptTicketClosure', () => {
    it('should accept ticket closure', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Closure accepted' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await acceptTicketClosure(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/accept-closure`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('rejectTicketClosure', () => {
    it('should reject ticket closure', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Closure rejected' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await rejectTicketClosure(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/reject-closure`)
      expect(result).toEqual(mockResponse)
    })
  })

  describe('forceCloseTicket', () => {
    it('should force close ticket', async () => {
      const ticketId = 10
      const mockResponse = { message: 'Ticket force closed' }

      api.post.mockResolvedValue({ data: mockResponse })

      const result = await forceCloseTicket(ticketId)

      expect(api.post).toHaveBeenCalledWith(`/tickets/${ticketId}/force-close`)
      expect(result).toEqual(mockResponse)
    })
  })
})
