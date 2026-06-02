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

export async function getUserStatistics(userId) {
  const response = await api.get(`/users/${userId}/statistics`)
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

// PB-51: User Account Management API calls
export async function createUser(userData) {
  const response = await api.post('/users', userData)
  return response.data
}

export async function updateUserDetails(userId, userData) {
  const response = await api.put(`/users/${userId}`, userData)
  return response.data
}

export async function deactivateUser(userId) {
  const response = await api.put(`/users/${userId}/deactivate`)
  return response.data
}

export async function reactivateUser(userId) {
  const response = await api.put(`/users/${userId}/reactivate`)
  return response.data
}

export async function getUsersList(params) {
  const response = await api.get('/users/list', { params })
  return response.data
}

export async function getAgentTeams() {
  const response = await api.get('/users/agent-teams')
  return response.data
}

export async function setMyAvailability(availability) {
  const response = await api.put('/users/me/availability', { availability })
  return response.data
}
