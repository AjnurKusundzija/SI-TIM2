import api from './api'

export async function login(email, password) {
  const response = await api.post('/auth/login', { email, password })
  const data = response.data
  sessionStorage.setItem('accessToken', data.accessToken)
  sessionStorage.setItem('refreshToken', data.refreshToken)
  sessionStorage.setItem('user', JSON.stringify({
    userId: data.userId,
    firstName: data.firstName,
    lastName: data.lastName,
    email: data.email,
    role: data.role,
  }))
  return data
}

export async function logout() {
  const refreshToken = sessionStorage.getItem('refreshToken')
  if (refreshToken) {
    try {
      await api.post('/auth/logout', { refreshToken })
    } catch {
      // token already expired/revoked — still clear session
    }
  }
  sessionStorage.clear()
}

export function getUser() {
  const raw = sessionStorage.getItem('user')
  return raw ? JSON.parse(raw) : null
}

export function isAuthenticated() {
  return !!sessionStorage.getItem('accessToken')
}
