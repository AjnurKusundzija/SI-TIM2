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
// US-53: Dohvati otvorene tikete dodijeljene agentu
export async function getOpenAssignedTickets() {
  const response = await api.get('/tickets/assigned/open')
  return response.data
}

// US-54: Dohvati zatvorene tikete koji su bili dodijeljeni agentu
export async function getClosedAssignedTickets() {
  const response = await api.get('/tickets/assigned/closed')
  return response.data
}

// US-56: Dohvati listu dostupnih agenata sa score-ovima za prosljeđivanje
export async function getAgentScores(ticketId) {
  const response = await api.get(`/tickets/${ticketId}/forward/agents`)
  return response.data
}

// US-55: Automatski proslijedi tiket agentu s najvišim score-om
export async function autoForwardTicket(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/forward/auto`)
  return response.data
}

// US-56: Proslijedi tiket konkretnom odabranom agentu
export async function forwardTicketToAgent(ticketId, targetAgentId) {
  const response = await api.post(`/tickets/${ticketId}/forward/agent`, { targetAgentId })
  return response.data
}

// US-TechnicianForwarding: Proslijedi tiket tehničaru na određenoj lokaciji
export async function forwardTicketToTechnician(ticketId, location) {
  const response = await api.post(`/tickets/${ticketId}/forward/technician`, { location })
  return response.data
}

// Internal Priority Management
export async function updateInternalPriority(ticketId, priority) {
  const response = await api.post(`/tickets/${ticketId}/internal-priority`, { priority })
  return response.data
}
