// PB-51 / US-74, US-75, US-89 — Frontend testovi liste korisnika
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getUsersList: vi.fn(),
  useAuth: vi.fn(),
  navigate: vi.fn(),
}))

vi.mock('../services/userService', () => ({
  getUsersList: mocks.getUsersList,
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

import UsersList from '../pages/UsersList'

const ADMIN = { role: 'ADMINISTRATOR' }
const AGENT = { role: 'AGENT' }

function renderAt(path) {
  return render(
    <MemoryRouter initialEntries={[path]}>
      <UsersList />
    </MemoryRouter>,
  )
}

const SAMPLE_USERS = [
  { userId: 1, firstName: 'Marko', lastName: 'Markovic', email: 'marko@t.ba', phone: '061111111', location: 'SARAJEVO', role: 'CLIENT', accountStatus: 'ACTIVE', expertiseCategory: '' },
  { userId: 2, firstName: 'Iva', lastName: 'Ivic', email: 'iva@t.ba', phone: '061222222', location: 'MOSTAR', role: 'AGENT', accountStatus: 'ACTIVE', expertiseCategory: 'INTERNET' },
]

describe('UsersList — Klijenti (PB-51 / US-75)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: ADMIN })
    mocks.getUsersList.mockResolvedValue({ users: [SAMPLE_USERS[0]], totalCount: 1, totalPages: 1, page: 1, pageSize: 10 })
  })

  it('zove API sa role=CLIENT i status=ACTIVE', async () => {
    renderAt('/users/clients')
    await waitFor(() =>
      expect(mocks.getUsersList).toHaveBeenCalledWith(expect.objectContaining({
        role: 'CLIENT',
        status: 'ACTIVE',
      })),
    )
  })

  it('prikazuje pretraga input po imenu/emailu/telefonu', () => {
    renderAt('/users/clients')
    expect(screen.getByPlaceholderText(/Pretraži po imenu, emailu ili telefonu/i)).toBeInTheDocument()
  })

  it('pretraga prosljeđuje search parametar API-ju', async () => {
    renderAt('/users/clients')
    await waitFor(() => expect(mocks.getUsersList).toHaveBeenCalled())
    mocks.getUsersList.mockClear()
    fireEvent.change(screen.getByPlaceholderText(/Pretraži po imenu/i), { target: { value: 'Marko' } })
    await waitFor(() =>
      expect(mocks.getUsersList).toHaveBeenCalledWith(expect.objectContaining({ search: 'Marko' })),
    )
  })

  it('prikazuje detalji button za otvaranje korisnika', async () => {
    renderAt('/users/clients')
    await waitFor(() => expect(screen.getByText('Marko Markovic')).toBeInTheDocument())
    expect(screen.getAllByText('Detalji').length).toBeGreaterThan(0)
  })
})

describe('UsersList — Agenti (PB-51 / US-89)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: ADMIN })
    mocks.getUsersList.mockResolvedValue({ users: [SAMPLE_USERS[1]], totalCount: 1, totalPages: 1, page: 1, pageSize: 10 })
  })

  it('zove API sa role=AGENT i status=ACTIVE', async () => {
    renderAt('/users/agents')
    await waitFor(() =>
      expect(mocks.getUsersList).toHaveBeenCalledWith(expect.objectContaining({ role: 'AGENT', status: 'ACTIVE' })),
    )
  })

  it('prikazuje kolonu Ekspertiza i ekspertiza vrijednost', async () => {
    renderAt('/users/agents')
    await waitFor(() => expect(screen.getByText('Ekspertiza')).toBeInTheDocument())
    expect(screen.getByText('INTERNET')).toBeInTheDocument()
  })

  it('agent koji posjeti /users/agents biva preusmjeren na /dashboard', () => {
    mocks.useAuth.mockReturnValue({ user: AGENT })
    renderAt('/users/agents')
    expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
  })

  it('agent koji posjeti /users/deactivated biva preusmjeren na /dashboard', () => {
    mocks.useAuth.mockReturnValue({ user: AGENT })
    renderAt('/users/deactivated')
    expect(mocks.navigate).toHaveBeenCalledWith('/dashboard')
  })
})

describe('UsersList — prazno stanje i filteri (PB-51 / US-74)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: ADMIN })
  })

  it('prazna lista prikazuje informativnu poruku', async () => {
    mocks.getUsersList.mockResolvedValue({ users: [], totalCount: 0, totalPages: 1, page: 1, pageSize: 10 })
    renderAt('/users/clients')
    await waitFor(() => expect(screen.getByText(/Nema pronađenih korisnika/i)).toBeInTheDocument())
  })

  it('filter po lokaciji prosljeđuje location parametar', async () => {
    mocks.getUsersList.mockResolvedValue({ users: SAMPLE_USERS, totalCount: 2, totalPages: 1, page: 1, pageSize: 10 })
    renderAt('/users/clients')
    await waitFor(() => expect(mocks.getUsersList).toHaveBeenCalled())
    mocks.getUsersList.mockClear()
    const selects = document.querySelectorAll('select')
    // Pronađi location filter
    const locationSelect = Array.from(selects).find(s => Array.from(s.options).some(o => o.value === 'SARAJEVO'))
    expect(locationSelect).toBeTruthy()
    fireEvent.change(locationSelect, { target: { value: 'MOSTAR' } })
    await waitFor(() =>
      expect(mocks.getUsersList).toHaveBeenCalledWith(expect.objectContaining({ location: 'MOSTAR' })),
    )
  })
})
