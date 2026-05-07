import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({ user: null }))

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: mocks.user }),
}))

import ProtectedRoute from '../components/layout/ProtectedRoute'

describe('ProtectedRoute (layout — US-1, US-2)', () => {
  beforeEach(() => {
    mocks.user = null
    vi.clearAllMocks()
  })

  // US-2: neautentifikovani korisnik se preusmjerava na /login
  it('redirects to /login when user is not authenticated', () => {
    render(
      <MemoryRouter initialEntries={['/dashboard']}>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </MemoryRouter>
    )
    expect(screen.queryByText('Protected Content')).not.toBeInTheDocument()
  })

  // US-1: autentifikovani korisnik vidi zaštićeni sadržaj
  it('renders children when user is authenticated', () => {
    mocks.user = { role: 'CLIENT' }
    render(
      <MemoryRouter>
        <ProtectedRoute>
          <div>Protected Content</div>
        </ProtectedRoute>
      </MemoryRouter>
    )
    expect(screen.getByText('Protected Content')).toBeInTheDocument()
  })

  // Sigurnosno: korisnik s pogrešnom ulogom se preusmjerava na /dashboard
  it('redirects to /dashboard when user role is not in allowedRoles', () => {
    mocks.user = { role: 'CLIENT' }
    render(
      <MemoryRouter>
        <ProtectedRoute allowedRoles={['AGENT', 'ADMINISTRATOR']}>
          <div>Agent Only</div>
        </ProtectedRoute>
      </MemoryRouter>
    )
    expect(screen.queryByText('Agent Only')).not.toBeInTheDocument()
  })

  // Sigurnosno: korisnik s ispravnom ulogom vidi sadržaj
  it('renders children when user role is in allowedRoles', () => {
    mocks.user = { role: 'AGENT' }
    render(
      <MemoryRouter>
        <ProtectedRoute allowedRoles={['AGENT', 'ADMINISTRATOR']}>
          <div>Agent Content</div>
        </ProtectedRoute>
      </MemoryRouter>
    )
    expect(screen.getByText('Agent Content')).toBeInTheDocument()
  })
})