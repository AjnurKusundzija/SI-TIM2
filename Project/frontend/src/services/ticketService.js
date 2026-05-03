import api from './api'

//  US-11: Dohvati tikete prijavljenog korisnika
export async function getMyTickets() {
  const response = await api.get('/ticket/my-tickets')
  return response.data
}

// PB-22: Kreiraj novi tiket
export async function createTicket(ticketData) {
  const response = await api.post('/ticket', ticketData)
  return response.data
}