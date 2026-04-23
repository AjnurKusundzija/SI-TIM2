import api from './api'

export async function login(email, password) {
  const response = await api.post('/auth/login', { email, password })
  const data = response.data
  sessionStorage.setItem('token', data.token)
  sessionStorage.setItem('user', JSON.stringify({
    userId: data.userId,
    firstName: data.firstName,
    lastName: data.lastName,
    email: data.email,
    role: data.role,
  }))
  return data
}

export function logout() {
  sessionStorage.removeItem('token')
  sessionStorage.removeItem('user')
}

export function getUser() {
  const raw = sessionStorage.getItem('user')
  return raw ? JSON.parse(raw) : null
}

export function isAuthenticated() {
  return !!sessionStorage.getItem('token')
}
