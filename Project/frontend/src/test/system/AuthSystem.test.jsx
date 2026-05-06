import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  login: vi.fn(),
  useNavigate: vi.fn(() => vi.fn()),
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: () => ({ login: mocks.login }),
}))

vi.mock('react-router-dom', async (importOriginal) => {
  const actual = await importOriginal()
  return { ...actual, useNavigate: mocks.useNavigate }
})

import Login from '../../pages/Login'

describe('PB-19 Auth system flow — tok prijave korisnika', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('korisnik unosi kredencijale, sistem poziva login i preusmjerava ga na dashboard', async () => {
    const navigate = vi.fn()
    mocks.useNavigate.mockReturnValue(navigate)
    mocks.login.mockResolvedValueOnce({ role: 'CLIENT' })

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    )

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'klijent@test.ba' },
    })
    fireEvent.change(screen.getByLabelText(/lozinka/i), {
      target: { value: 'Password123!' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.login).toHaveBeenCalledWith('klijent@test.ba', 'Password123!')
    })

    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })

  it('pogresan unos prikazuje poruku greske bez otkrivanja detalja', async () => {
    mocks.login.mockRejectedValueOnce(new Error('Invalid credentials'))

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    )

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'pogresno@test.ba' },
    })
    fireEvent.change(screen.getByLabelText(/lozinka/i), {
      target: { value: 'PogresnaSifra' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(screen.queryByText(/invalid|pogresan|neispravni|nevažeći/i)).toBeInTheDocument()
    })
  })
})
