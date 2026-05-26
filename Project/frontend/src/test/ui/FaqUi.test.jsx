import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  getFaqs: vi.fn(),
  getAllFaqs: vi.fn(),
  useAuth: vi.fn(() => ({ user: { role: 'CLIENT' } })),
}))

vi.mock('../../services/faqService', () => ({
  getFaqs: mocks.getFaqs,
  getAllFaqs: mocks.getAllFaqs,
  createFaq: vi.fn(),
  updateFaq: vi.fn(),
  deleteFaq: vi.fn(),
}))

vi.mock('../../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

import Faq from '../../pages/Faq'

describe('PB-47 FAQ UI', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('renders FAQ cards for available content', async () => {
    mocks.getFaqs.mockResolvedValueOnce([
      {
        faqId: 1,
        question: 'Kako resetovati ruter?',
        answer: 'Restartujte ruter.',
        category: 'Internet',
        sortOrder: 1,
      },
      {
        faqId: 2,
        question: 'Kako provjeriti racun?',
        answer: 'Provjerite racun kroz korisnicki portal.',
        category: 'Racuni',
        sortOrder: 2,
      },
    ])

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    expect(screen.getAllByRole('article')).toHaveLength(2)
    expect(screen.getByText('Internet')).toBeInTheDocument()
    expect(screen.getByText('Racuni')).toBeInTheDocument()
  })

  it('renders a fallback message when there is no FAQ content', async () => {
    mocks.getFaqs.mockResolvedValueOnce([])

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Nema FAQ pitanja')).toBeInTheDocument())
    expect(screen.getByText(/trenutno nema dostupnih odgovora/i)).toBeInTheDocument()
  })
})
