import api from './api'

// PB-42: Dohvati statistiku rada za prijavljenog agenta ili tehničara
export async function getMyStatistics() {
  const response = await api.get('/users/me/statistics')
  return response.data
}

// Dashboard: Dohvati 5 najrecentnijih tiketa za agenta ili tehničara
export async function getMyRecentTickets() {
  const response = await api.get('/users/me/recent-tickets')
  return response.data
}
