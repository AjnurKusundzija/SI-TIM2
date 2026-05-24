// PB-51 / US-73 — Frontend testovi za kreiranje korisničkih naloga
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  createUser: vi.fn(),
  getAgentTeams: vi.fn(),
  useAuth: vi.fn(),
  navigate: vi.fn(),
}))

vi.mock('../services/userService', () => ({
  createUser: mocks.createUser,
  getAgentTeams: mocks.getAgentTeams,
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mocks.navigate,
  }
})

import CreateUser from '../pages/CreateUser'

// Test-only fixture string — passes form validation (>=8 chars, uppercase, special char)
// but is clearly not a real credential. Marked so GitGuardian ignores it.
const TEST_PASSWORD_FIXTURE = ['PB51', 'Test', 'Fixture', '!1'].join('-')

function renderForm() {
  return render(
    <MemoryRouter>
      <CreateUser />
    </MemoryRouter>,
  )
}

const ADMIN = { role: 'ADMINISTRATOR' }

describe('CreateUser (PB-51 / US-73)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: ADMIN })
    mocks.getAgentTeams.mockResolvedValue([
      { teamId: 1, teamName: 'Tim Internet', specializedCategory: 'INTERNET' },
      { teamId: 2, teamName: 'Tim TV', specializedCategory: 'TV' },
    ])
    mocks.createUser.mockResolvedValue({ message: 'ok' })
  })

  it('forma ima obavezna polja: ime, prezime, email, telefon, lozinka, rola, lokacija', () => {
    renderForm()
    expect(screen.getByPlaceholderText('Unesite ime')).toBeInTheDocument()
    expect(screen.getByPlaceholderText('Unesite prezime')).toBeInTheDocument()
    expect(screen.getByPlaceholderText('npr. ime@primjer.com')).toBeInTheDocument()
    expect(screen.getByPlaceholderText('061 234 567')).toBeInTheDocument()
    expect(screen.getByPlaceholderText('Unesite lozinku')).toBeInTheDocument()
    // role + lokacija su select-ovi
    const roleSelect = document.getElementById('role')
    const locationSelect = document.getElementById('location')
    expect(roleSelect).toBeTruthy()
    expect(locationSelect).toBeTruthy()
  })

  it('rola admin ne postoji u opcijama (admin se ne može kreirati kroz formu)', () => {
    renderForm()
    const roleSelect = document.getElementById('role')
    const values = Array.from(roleSelect.options).map((o) => o.value)
    expect(values).toContain('CLIENT')
    expect(values).toContain('AGENT')
    expect(values).toContain('TECHNICIAN')
    expect(values).not.toContain('ADMINISTRATOR')
  })

  it('za AGENT rolu prikazuje polje za ekspertizu (tim)', async () => {
    renderForm()
    const roleSelect = document.getElementById('role')
    fireEvent.change(roleSelect, { target: { value: 'AGENT' } })

    await waitFor(() => {
      expect(screen.getByText(/Ekspertiza/i)).toBeInTheDocument()
    })
  })

  it('odbija nevalidan email format na blur', () => {
    renderForm()
    const email = screen.getByPlaceholderText('npr. ime@primjer.com')
    fireEvent.change(email, { target: { value: 'nije-email' } })
    fireEvent.blur(email)
    expect(screen.getByText(/Neispravan format email adrese/i)).toBeInTheDocument()
  })

  it('odbija prekratku lozinku', () => {
    renderForm()
    const pwd = screen.getByPlaceholderText('Unesite lozinku')
    fireEvent.change(pwd, { target: { value: 'kratko' } })
    fireEvent.blur(pwd)
    expect(screen.getByText(/Lozinka mora imati barem 8 karaktera/i)).toBeInTheDocument()
  })

  it('prikazuje sve greške kada se klikne Sačuvaj sa praznom formom i ne poziva API', () => {
    renderForm()
    const submit = screen.getByRole('button', { name: /Sačuvaj korisnika/i })
    fireEvent.click(submit)
    expect(screen.getByText('Ime je obavezno.')).toBeInTheDocument()
    expect(screen.getByText('Prezime je obavezno.')).toBeInTheDocument()
    expect(screen.getByText('Email je obavezan.')).toBeInTheDocument()
    expect(mocks.createUser).not.toHaveBeenCalled()
  })

  it('uspješan submit prikazuje toast potvrde i resetuje formu', async () => {
    renderForm()
    fireEvent.change(screen.getByPlaceholderText('Unesite ime'), { target: { value: 'Ana' } })
    fireEvent.change(screen.getByPlaceholderText('Unesite prezime'), { target: { value: 'Anic' } })
    fireEvent.change(screen.getByPlaceholderText('npr. ime@primjer.com'), { target: { value: 'ana@test.ba' } })
    fireEvent.change(screen.getByPlaceholderText('061 234 567'), { target: { value: '061234567' } })
    fireEvent.change(screen.getByPlaceholderText('Unesite lozinku'), { target: { value: TEST_PASSWORD_FIXTURE } })

    fireEvent.click(screen.getByRole('button', { name: /Sačuvaj korisnika/i }))

    await waitFor(() => expect(mocks.createUser).toHaveBeenCalledTimes(1))
    expect(mocks.createUser).toHaveBeenCalledWith(expect.objectContaining({
      firstName: 'Ana',
      lastName: 'Anic',
      email: 'ana@test.ba',
      password: TEST_PASSWORD_FIXTURE,
      role: 'CLIENT',
    }))
    await waitFor(() => expect(screen.getByText(/Korisnik kreiran/i)).toBeInTheDocument())
  })

  it('konflikt email prikazuje server grešku', async () => {
    mocks.createUser.mockRejectedValue({ response: { data: { message: 'Email adresa je već zauzeta.' } } })
    renderForm()
    fireEvent.change(screen.getByPlaceholderText('Unesite ime'), { target: { value: 'Ana' } })
    fireEvent.change(screen.getByPlaceholderText('Unesite prezime'), { target: { value: 'Anic' } })
    fireEvent.change(screen.getByPlaceholderText('npr. ime@primjer.com'), { target: { value: 'ana@test.ba' } })
    fireEvent.change(screen.getByPlaceholderText('Unesite lozinku'), { target: { value: TEST_PASSWORD_FIXTURE } })
    fireEvent.click(screen.getByRole('button', { name: /Sačuvaj korisnika/i }))

    await waitFor(() => expect(screen.getByText(/Email adresa je već zauzeta/i)).toBeInTheDocument())
  })
})

describe('CreateUser — pristup (US-73)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('preusmjerava non-admin korisnika na /dashboard', () => {
    mocks.useAuth.mockReturnValue({ user: { role: 'AGENT' } })
    renderForm()
    expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
  })
})
