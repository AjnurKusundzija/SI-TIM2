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

// US-14, US-30: Dohvati detalje jednog tiketa — backend provjerava pristup prema roli
export async function getTicketById(ticketId) {
  const response = await api.get(`/ticket/${ticketId}`)
  return response.data
}

// US-15: Dohvati historiju komentara za tiket
// Koristi se pri učitavanju stranice; real-time poruke će ići kroz SignalR (PB-27)
export async function getTicketComments(ticketId) {
  const response = await api.get(`/comment/ticket/${ticketId}`)
  return response.data
}
