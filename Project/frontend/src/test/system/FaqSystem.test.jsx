import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

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

describe('PB-47 FAQ system flow', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('allows a user to open the FAQ page and read questions and answers without an error', async () => {
    mocks.getFaqs.mockResolvedValueOnce([
      {
        faqId: 1,
        question: 'Kako resetovati ruter?',
        answer: 'Iskljucite ruter 30 sekundi i ponovo ga ukljucite.',
        category: 'Internet',
        sortOrder: 1,
      },
      {
        faqId: 2,
        question: 'Kako otvoriti novi tiket?',
        answer: 'Izaberite Kreiraj tiket i popunite formu.',
        category: 'Tiketi',
        sortOrder: 2,
      },
    ])

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())
    expect(screen.getByText('Kako otvoriti novi tiket?')).toBeInTheDocument()

    fireEvent.click(screen.getByRole('button', { name: /kako resetovati ruter/i }))

    expect(screen.getByText('Iskljucite ruter 30 sekundi i ponovo ga ukljucite.')).toBeInTheDocument()
    expect(consoleErrorSpy).not.toHaveBeenCalled()
  })
})
