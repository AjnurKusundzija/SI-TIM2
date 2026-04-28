import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, act } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  login: vi.fn(),
  logout: vi.fn(),
  getUser: vi.fn(() => null),
  isAuthenticated: vi.fn(() => false),
}))

vi.mock('../services/authService', () => ({
  login: mocks.login,
  logout: mocks.logout,
  getUser: mocks.getUser,
  isAuthenticated: mocks.isAuthenticated,
}))

import { AuthProvider, useAuth } from '../context/AuthContext'

function TestConsumer() {
  const { user, login, logout } = useAuth()
  return (
    <div>
      <span data-testid="user">{user ? user.email : 'none'}</span>
      <button onClick={() => login('a@b.com', 'pass')}>login</button>
      <button onClick={logout}>logout</button>
    </div>
  )
}

describe('AuthContext', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getUser.mockReturnValue(null)
  })

  // US-1: inicijalno stanje — nema korisnika dok nije prijavljen
  it('provides null user when sessionStorage is empty', () => {
    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>
    )

    expect(screen.getByTestId('user').textContent).toBe('none')
  })

  // US-1: login ažurira korisničko stanje u kontekstu
  it('login() updates user state in context', async () => {
    const userData = {
      userId: 1,
      firstName: 'Emir',
      lastName: 'Hadzic',
      email: 'emir@example.com',
      role: 'CLIENT',
      accessToken: 'jwt',
      refreshToken: 'rt',
    }
    mocks.login.mockResolvedValueOnce(userData)

    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>
    )

    await act(async () => {
      screen.getByText('login').click()
    })

    expect(screen.getByTestId('user').textContent).toBe('emir@example.com')
  })

  // US-2: logout briše korisnika iz konteksta
  it('logout() clears user state in context', async () => {
    const userData = {
      userId: 1,
      firstName: 'Emir',
      lastName: 'Hadzic',
      email: 'emir@example.com',
      role: 'CLIENT',
      accessToken: 'jwt',
      refreshToken: 'rt',
    }
    mocks.login.mockResolvedValueOnce(userData)
    mocks.logout.mockResolvedValueOnce(undefined)

    render(
      <AuthProvider>
        <TestConsumer />
      </AuthProvider>
    )

    await act(async () => {
      screen.getByText('login').click()
    })

    expect(screen.getByTestId('user').textContent).toBe('emir@example.com')

    await act(async () => {
      screen.getByText('logout').click()
    })

    expect(screen.getByTestId('user').textContent).toBe('none')
  })
})
