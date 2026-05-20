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

// PB-20: Dohvati podatke o profilu prijavljenog korisnika
export async function getMyProfile() {
  const response = await api.get('/users/me')
  return response.data
}

export async function getUserProfile(userId) {
  const response = await api.get(`/users/${userId}`)
  return response.data
}

export async function updateEmail(email) {
  const response = await api.put('/users/me/email', { email })
  return response.data
}

export async function updatePassword(currentPassword, newPassword, confirmPassword) {
  const response = await api.put('/users/me/password', {
    currentPassword,
    newPassword,
    confirmPassword,
  })
  return response.data
}
