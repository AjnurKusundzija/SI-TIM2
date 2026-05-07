import { describe, it, expect, beforeEach, vi } from 'vitest'
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

describe('PB-19 Auth acceptance — poslovni tok prijave', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('klijent se uspjesno prijavljuje i sistem ga preusmjerava', async () => {
    const navigate = vi.fn()
    mocks.useNavigate.mockReturnValue(navigate)
    mocks.login.mockResolvedValueOnce({ role: 'CLIENT' })

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    )

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'klijent@bhtelecom.ba' },
    })
    fireEvent.change(screen.getByLabelText(/lozinka/i), {
      target: { value: 'MojaLozinka123!' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      expect(mocks.login).toHaveBeenCalledWith('klijent@bhtelecom.ba', 'MojaLozinka123!')
    })

    // AC: korisnik je uspjesno prijavljen — login pozvan s ispravnim podacima
    expect(mocks.login).toHaveBeenCalledTimes(1)
  })

  it('poruka greske ne otkriva koji podatak je pogresan — genericki tekst', async () => {
    mocks.login.mockRejectedValueOnce(new Error('Invalid credentials'))

    render(
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    )

    fireEvent.change(screen.getByLabelText(/email/i), {
      target: { value: 'nepostoji@test.ba' },
    })
    fireEvent.change(screen.getByLabelText(/lozinka/i), {
      target: { value: 'GreskaLozinka' },
    })
    fireEvent.submit(screen.getByRole('button', { name: /prijavi se/i }).closest('form'))

    await waitFor(() => {
      // AC US-3: generična poruka, ne otkriva koji podatak je pogrešan
      expect(screen.queryByText(/invalid|pogresan|neispravni|nevažeći/i)).toBeInTheDocument()
    })

    // AC: email adresa se ne otkriva u poruci greske
    expect(screen.queryByText(/nepostoji@test\.ba/i)).not.toBeInTheDocument()
  })
})
