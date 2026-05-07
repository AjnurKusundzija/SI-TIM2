import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  user: null,
  navigate: vi.fn(),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: mocks.user }),
}))

vi.mock('react-router-dom', () => ({
  useNavigate: () => mocks.navigate,
}))

import Dashboard from '../pages/Dashboard'

describe('Dashboard', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.user = null
  })

  it('shows welcome message with user firstName', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Dashboard />)
    expect(screen.getByText(/Dobrodošli nazad, Emir!/i)).toBeInTheDocument()
  })

  it('shows CLIENT cards (Moji tiketi + Kreiraj tiket) for CLIENT role', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Dashboard />)
    expect(screen.getByText('Moji tiketi')).toBeInTheDocument()
    expect(screen.getByText('Kreiraj tiket')).toBeInTheDocument()
  })

  it('does not show Kreiraj tiket for AGENT role', () => {
    mocks.user = { firstName: 'Haris', role: 'AGENT' }
    render(<Dashboard />)
    expect(screen.getByText('Tiketi')).toBeInTheDocument()
    expect(screen.queryByText('Kreiraj tiket')).not.toBeInTheDocument()
  })

  it('shows correct role description for CLIENT', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Dashboard />)
    expect(screen.getByText(/pregled vašeg korisničkog računa/i)).toBeInTheDocument()
  })

  it('shows correct role description for AGENT', () => {
    mocks.user = { firstName: 'Haris', role: 'AGENT' }
    render(<Dashboard />)
    expect(screen.getByText(/tiketi na čekanju/i)).toBeInTheDocument()
  })

  it('shows correct role description for TECHNICIAN', () => {
    mocks.user = { firstName: 'Amer', role: 'TECHNICIAN' }
    render(<Dashboard />)
    expect(screen.getByText(/dodijeljeni tiketi i zadaci/i)).toBeInTheDocument()
  })

  it('shows correct role description for ADMINISTRATOR', () => {
    mocks.user = { firstName: 'Admin', role: 'ADMINISTRATOR' }
    render(<Dashboard />)
    expect(screen.getByText(/sistemski pregled/i)).toBeInTheDocument()
  })
})
