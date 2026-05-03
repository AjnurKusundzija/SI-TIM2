import { describe, it, expect, beforeEach, vi, afterEach } from 'vitest'

vi.mock('../services/api', () => ({
  default: {
    post: vi.fn(),
  },
}))

import api from '../services/api'
import { login, logout, getUser, isAuthenticated } from '../services/authService'

const FAKE_RESPONSE = {
  accessToken: 'fake-jwt',
  refreshToken: 'fake-rt',
  userId: 7,
  firstName: 'Emir',
  lastName: 'Hadzic',
  email: 'emir@example.com',
  role: 'CLIENT',
}

describe('authService', () => {
  beforeEach(() => {
    sessionStorage.clear()
    vi.clearAllMocks()
  })

  afterEach(() => {
    sessionStorage.clear()
  })

  // ─── login ──────────────────────────────────────────────────────────────

  // US-1: uspješna prijava pohranjuje tokene u sessionStorage
  it('login() stores accessToken and refreshToken in sessionStorage', async () => {
    api.post.mockResolvedValueOnce({ data: FAKE_RESPONSE })

    await login('emir@example.com', 'Password123!')

    expect(sessionStorage.getItem('accessToken')).toBe(FAKE_RESPONSE.accessToken)
    expect(sessionStorage.getItem('refreshToken')).toBe(FAKE_RESPONSE.refreshToken)
  })

  // US-1: korisnički podaci se pohranjuju u sessionStorage
  it('login() stores user data in sessionStorage', async () => {
    api.post.mockResolvedValueOnce({ data: FAKE_RESPONSE })

    await login('emir@example.com', 'Password123!')

    const stored = JSON.parse(sessionStorage.getItem('user'))
    expect(stored.userId).toBe(FAKE_RESPONSE.userId)
    expect(stored.email).toBe(FAKE_RESPONSE.email)
    expect(stored.role).toBe(FAKE_RESPONSE.role)
  })

  // US-1: login vraća podatke iz API odgovora
  it('login() returns data from API response', async () => {
    api.post.mockResolvedValueOnce({ data: FAKE_RESPONSE })

    const result = await login('emir@example.com', 'Password123!')

    expect(result.accessToken).toBe(FAKE_RESPONSE.accessToken)
    expect(result.userId).toBe(FAKE_RESPONSE.userId)
  })

  // US-1: poziv API-a sa ispravnim endpointom i tijelom
  it('login() calls POST /auth/login with email and password', async () => {
    api.post.mockResolvedValueOnce({ data: FAKE_RESPONSE })

    await login('emir@example.com', 'Password123!')

    expect(api.post).toHaveBeenCalledWith('/auth/login', {
      email: 'emir@example.com',
      password: 'Password123!',
    })
  })

  // ─── logout ─────────────────────────────────────────────────────────────

  // US-2: odjava briše sessionStorage
  it('logout() clears sessionStorage', async () => {
    sessionStorage.setItem('accessToken', 'token')
    sessionStorage.setItem('refreshToken', 'rt')
    sessionStorage.setItem('user', JSON.stringify({ userId: 1 }))
    api.post.mockResolvedValueOnce({})

    await logout()

    expect(sessionStorage.getItem('accessToken')).toBeNull()
    expect(sessionStorage.getItem('refreshToken')).toBeNull()
    expect(sessionStorage.getItem('user')).toBeNull()
  })

  // US-2: odjava poziva API s refresh tokenom
  it('logout() calls POST /auth/logout with refreshToken', async () => {
    sessionStorage.setItem('refreshToken', 'my-rt')
    api.post.mockResolvedValueOnce({})

    await logout()

    expect(api.post).toHaveBeenCalledWith('/auth/logout', { refreshToken: 'my-rt' })
  })

  // US-2: sessionStorage se briše čak i ako API poziv ne uspije
  it('logout() clears sessionStorage even when API throws', async () => {
    sessionStorage.setItem('accessToken', 'token')
    sessionStorage.setItem('refreshToken', 'rt')
    api.post.mockRejectedValueOnce(new Error('Network error'))

    await logout()

    expect(sessionStorage.getItem('accessToken')).toBeNull()
  })

  // US-2: ako nema refresh tokena, API se ne poziva
  it('logout() skips API call when no refreshToken in session', async () => {
    await logout()

    expect(api.post).not.toHaveBeenCalled()
  })

  // ─── getUser ────────────────────────────────────────────────────────────

  it('getUser() returns null when sessionStorage is empty', () => {
    expect(getUser()).toBeNull()
  })

  it('getUser() returns parsed user object from sessionStorage', () => {
    const user = { userId: 7, email: 'emir@example.com', role: 'CLIENT' }
    sessionStorage.setItem('user', JSON.stringify(user))

    expect(getUser()).toEqual(user)
  })

  // ─── isAuthenticated ─────────────────────────────────────────────────────

  // US-2: nije autentifikovan kada nema tokena (npr. nakon odjave)
  it('isAuthenticated() returns false when no accessToken', () => {
    expect(isAuthenticated()).toBe(false)
  })

  // US-1: autentifikovan kad token postoji u sessionStorage
  it('isAuthenticated() returns true when accessToken present', () => {
    sessionStorage.setItem('accessToken', 'some-jwt')

    expect(isAuthenticated()).toBe(true)
  })
})
