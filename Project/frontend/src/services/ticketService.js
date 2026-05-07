import api from './api'

// US-11: Dohvati tikete prijavljenog korisnika
export async function getMyTickets() {
  const response = await api.get('/mytickets')
  return response.data
}

// PB-32: Dohvati sve tikete (ADMIN/AGENT/TECHNICIAN); assignedOnly=true → samo dodijeljeni (AGENT)
export async function getAllTickets(assignedOnly = false) {
  const response = await api.get('/tickets', { params: assignedOnly ? { assignedOnly: true } : {} })
  return response.data
}

// PB-22: Kreiraj novi tiket
export async function createTicket(ticketData) {
  const response = await api.post('/tickets', ticketData)
  return response.data
}

// US-14, US-30: Dohvati detalje jednog tiketa — backend provjerava pristup prema roli
export async function getTicketById(ticketId) {
  const response = await api.get(`/tickets/${ticketId}`)
  return response.data
}

// US-15: Dohvati historiju komentara za tiket
export async function getTicketComments(ticketId) {
  const response = await api.get(`/comment/tickets/${ticketId}`)
  return response.data
}

// PB-27: Dodaj novi komentar na tiket
export async function addComment(ticketId, content) {
  const response = await api.post(`/comment/tickets/${ticketId}`, { content })
  return response.data
}