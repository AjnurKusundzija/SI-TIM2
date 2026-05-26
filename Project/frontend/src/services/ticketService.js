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

// PB-56 / US-80: Kreiraj novi tiket sa attachment-ima
// onUploadProgress: callback(percent 0-100) — koristi se za prikaz progress indikatora.
export async function createTicketWithAttachments(ticketData, files, onUploadProgress) {
  const formData = new FormData()
  formData.append('Subject', ticketData.Subject)
  formData.append('Type', ticketData.Type)
  formData.append('Description', ticketData.Description)
  formData.append('Priority', ticketData.Priority)

  files.forEach((file) => {
    formData.append('Attachments', file)
  })

  const response = await api.post('/tickets/attachments', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    onUploadProgress: (event) => {
      if (!onUploadProgress) return
      const total = event.total || event.loaded || 1
      const percent = Math.round((event.loaded * 100) / total)
      onUploadProgress(percent)
    }
  })
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

// PB-56 / US-80: Dodaj novi komentar sa attachment-ima
export async function addCommentWithAttachments(ticketId, content, files, onUploadProgress) {
  const formData = new FormData()
  formData.append('Content', content)

  files.forEach((file) => {
    formData.append('Attachments', file)
  })

  const response = await api.post(`/comment/tickets/${ticketId}/attachments`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    onUploadProgress: (event) => {
      if (!onUploadProgress) return
      const total = event.total || event.loaded || 1
      const percent = Math.round((event.loaded * 100) / total)
      onUploadProgress(percent)
    }
  })
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

// US-TechnicianForwarding: Automatski proslijedi tiket tehničaru na lokaciji kreatora
export async function forwardTicketToTechnician(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/forward/technician`)
  return response.data
}

// PB-62 / US-105: Agent preuzima nedodijeljeni tiket sebi
export async function selfAssignTicket(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/self-assign`)
  return response.data
}

// Internal Priority Management
export async function updateInternalPriority(ticketId, priority) {
  const response = await api.post(`/tickets/${ticketId}/internal-priority`, { priority })
  return response.data
}

// PB-36 / US-60: Tehničar mijenja status tiketa koji mu je dodijeljen
export async function updateTicketStatus(ticketId, status) {
  const response = await api.post(`/tickets/${ticketId}/status`, { status })
  return response.data
}

// Closure Workflow
export async function closeTicket(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/close`)
  return response.data
}

export async function requestTicketClosure(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/request-closure`)
  return response.data
}

export async function acceptTicketClosure(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/accept-closure`)
  return response.data
}

export async function rejectTicketClosure(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/reject-closure`)
  return response.data
}

export async function forceCloseTicket(ticketId) {
  const response = await api.post(`/tickets/${ticketId}/force-close`)
  return response.data
}

// US-61: Dohvati ocjenu za tiket (null ako nije ocijenjen, 204 No Content)
export async function getTicketRating(ticketId) {
  const response = await api.get(`/tickets/${ticketId}/rating`)
  if (response.status === 204) return null
  return response.data
}

// US-61: Kreiraj ocjenu za tiket (samo CLIENT, tiket mora biti CLOSED)
export async function createTicketRating(ticketId, payload) {
  const response = await api.post(`/tickets/${ticketId}/rating`, payload)
  return response.data
}
