import api from './api'

// US-11: Dohvati tikete prijavljenog korisnika
export async function getMyTickets() {
  const response = await api.get('/ticket/my-tickets')
  return response.data
}

// PB-22: Kreiraj novi tiket
export async function createTicket(ticketData) {
  const response = await api.post('/ticket', ticketData)
  return response.data
}

// US-29: Dohvati sve tikete sa paginacijom (agent/administrator)
export async function getAllTickets(page = 1, pageSize = 20) {
  const response = await api.get('/ticket', { params: { page, pageSize } })
  return response.data
}

// US-30: Dohvati detalje jednog tiketa (agent/administrator)
export async function getTicketById(id) {
  const response = await api.get(`/ticket/${id}`)
  return response.data
}
