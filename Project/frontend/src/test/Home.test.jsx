import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, fireEvent } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  user: null,
  navigate: vi.fn(),
  on: vi.fn(),
  disconnect: vi.fn(),
}))

vi.mock('socket.io-client', () => ({
  io: () => ({ on: mocks.on, disconnect: mocks.disconnect }),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: mocks.user }),
}))

vi.mock('react-router-dom', () => ({
  useNavigate: () => mocks.navigate,
}))

import Home from '../pages/Home'

describe('Home page', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.user = null
  })

  it('renders the landing page heading', () => {
    render(<Home />)
    expect(screen.getByText(/Besprijekorna podrška/i)).toBeInTheDocument()
  })

  it('shows Prijava button when user is not authenticated', () => {
    render(<Home />)
    expect(screen.getByRole('button', { name: /prijava/i })).toBeInTheDocument()
  })

  it('shows Dashboard button when user is authenticated', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Home />)
    expect(screen.getByRole('button', { name: /dashboard/i })).toBeInTheDocument()
  })

  it('navigates to /login when Prijava button is clicked', () => {
    render(<Home />)
    fireEvent.click(screen.getByRole('button', { name: /prijava/i }))
    expect(mocks.navigate).toHaveBeenCalledWith('/login')
  })

  it('navigates to /dashboard when Dashboard button is clicked', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Home />)
    fireEvent.click(screen.getByRole('button', { name: /dashboard/i }))
    expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
  })

  it('CTA button navigates to /login when not authenticated', () => {
    render(<Home />)
    fireEvent.click(screen.getByRole('button', { name: /Započnite odmah/i }))
    expect(mocks.navigate).toHaveBeenCalledWith('/login')
  })

  it('CTA button navigates to /dashboard when authenticated', () => {
    mocks.user = { firstName: 'Emir', role: 'CLIENT' }
    render(<Home />)
    fireEvent.click(screen.getByRole('button', { name: /Započnite odmah/i }))
    expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
  })

  it('renders footer copyright', () => {
    render(<Home />)
    expect(screen.getByText(/Sva prava zadržana/i)).toBeInTheDocument()
  })
})
