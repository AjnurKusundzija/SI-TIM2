import { describe, it, expect, beforeEach, vi } from 'vitest'
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

describe('PB-47 FAQ acceptance', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('confirms FAQ content can help a user resolve a common issue before opening a ticket', async () => {
    mocks.getFaqs.mockResolvedValueOnce([
      {
        faqId: 1,
        question: 'Internet ne radi, sta mogu prvo provjeriti?',
        answer: 'Provjerite kablove, restartujte ruter i sacekajte da se veza ponovo uspostavi.',
        category: 'Internet',
        sortOrder: 1,
      },
    ])

    render(<Faq />)

    await waitFor(() => {
      expect(screen.getByText('Internet ne radi, sta mogu prvo provjeriti?')).toBeInTheDocument()
    })

    fireEvent.click(screen.getByRole('button', { name: /internet ne radi/i }))

    expect(screen.getByText(/provjerite kablove/i)).toBeInTheDocument()
    expect(screen.queryByText('Nema FAQ pitanja')).not.toBeInTheDocument()
  })
})
