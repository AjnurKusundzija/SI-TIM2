import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  login: vi.fn(),
  navigate: vi.fn(),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ login: mocks.login }),
}))

vi.mock('react-router-dom', () => ({
  useNavigate: () => mocks.navigate,
}))

import Login from '../pages/Login'

describe('Login page', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  function fillForm(email = 'user@example.com', password = 'Password123!') {
    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: email } })
    fireEvent.change(screen.getByLabelText(/lozinka/i), { target: { value: password } })
  }

  // US-1: forma prikazuje polja za email i lozinku
  it('renders email and password input fields', () => {
    render(<Login />)

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/lozinka/i)).toBeInTheDocument()
  })

  // US-1: uspješna prijava preusmjerava na /dashboard
  it('redirects to /dashboard after successful login', async () => {
    mocks.login.mockResolvedValueOnce(undefined)
    render(<Login />)

    fillForm()
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
    })
  })

  // US-3: neispravni kredencijali prikazuju generičku poruku greške
  it('shows generic error message when login fails', async () => {
    mocks.login.mockRejectedValueOnce(new Error('401'))
    render(<Login />)

    fillForm()
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      // US-3: generička poruka, ne otkriva koji je podatak pogrešan
      expect(screen.getByText(/nevažeći podaci/i)).toBeInTheDocument()
    })
  })

  // US-1: login se ne poziva kad su polja prazna (JS guard)
  it('does not call login when email or password is empty', async () => {
    render(<Login />)

    // Submit bez popunjavanja polja
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.login).not.toHaveBeenCalled()
    })
  })

  // US-119: login forma vizualno naznačava da se može unijeti email ili telefon
  it('label indicates email or phone number can be entered (US-119)', () => {
    render(<Login />)
    expect(screen.getByLabelText(/email adresa ili broj telefona/i)).toBeInTheDocument()
  })

  // US-119: placeholder pokazuje +387 format broja telefona
  it('placeholder shows +387 phone number format (US-119)', () => {
    render(<Login />)
    expect(screen.getByPlaceholderText(/\+387/i)).toBeInTheDocument()
  })

  // US-119: prijava sa brojem telefona (+387) poziva login servis
  it('calls login with phone number when +387 format is submitted (US-119)', async () => {
    mocks.login.mockResolvedValueOnce(undefined)
    render(<Login />)

    fireEvent.change(screen.getByLabelText(/email adresa ili broj telefona/i), {
      target: { value: '+38761234567' },
    })
    fireEvent.change(screen.getByLabelText(/lozinka/i), {
      target: { value: 'Password123!' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.login).toHaveBeenCalledWith('+38761234567', 'Password123!')
    })
  })

  // US-119: prijava sa emailom i dalje radi (korisnici bez broja telefona)
  it('still allows login with email address (US-119)', async () => {
    mocks.login.mockResolvedValueOnce(undefined)
    render(<Login />)

    fillForm('korisnik@firma.ba', 'Password123!')
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.login).toHaveBeenCalledWith('korisnik@firma.ba', 'Password123!')
    })
  })
})
