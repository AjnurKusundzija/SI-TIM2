import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  getFaqs: vi.fn(),
}))

vi.mock('../services/faqService', () => ({
  getFaqs: mocks.getFaqs,
}))

import Faq from '../pages/Faq'

const FAKE_FAQS = [
  {
    faqId: 1,
    question: 'Kako resetovati ruter?',
    answer: 'Isključite ruter 30 sekundi.',
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
]

function deferred() {
  let resolve
  let reject
  const promise = new Promise((res, rej) => {
    resolve = res
    reject = rej
  })

  return { promise, resolve, reject }
}

describe('Faq page', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('shows loading state while FAQ items are loading', () => {
    const request = deferred()
    mocks.getFaqs.mockReturnValueOnce(request.promise)

    render(<Faq />)

    expect(screen.getByRole('status', { name: /učitavanje pitanja/i })).toBeInTheDocument()
  })

  it('renders FAQ questions after loading', async () => {
    mocks.getFaqs.mockResolvedValueOnce(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())
    expect(screen.getByText('Kako otvoriti novi tiket?')).toBeInTheDocument()
    expect(screen.getByText('Internet')).toBeInTheDocument()
  })

  it('shows empty state when API returns no FAQ items', async () => {
    mocks.getFaqs.mockResolvedValueOnce([])

    render(<Faq />)

    await waitFor(() => {
      expect(screen.getByText('Nema FAQ pitanja')).toBeInTheDocument()
    })
  })

  it('shows error state and retries loading FAQ items', async () => {
    mocks.getFaqs
      .mockRejectedValueOnce(new Error('Network error'))
      .mockResolvedValueOnce(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => {
      expect(screen.getByText('Došlo je do greške')).toBeInTheDocument()
    })

    fireEvent.click(screen.getByRole('button', { name: /pokušaj ponovo/i }))

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())
    expect(mocks.getFaqs).toHaveBeenCalledTimes(2)
  })

  it('toggles FAQ answer when question is clicked', async () => {
    mocks.getFaqs.mockResolvedValueOnce(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())
    expect(screen.queryByText('Isključite ruter 30 sekundi.')).not.toBeInTheDocument()

    fireEvent.click(screen.getByRole('button', { name: /kako resetovati ruter/i }))

    expect(screen.getByText('Isključite ruter 30 sekundi.')).toBeInTheDocument()

    fireEvent.click(screen.getByRole('button', { name: /kako resetovati ruter/i }))

    expect(screen.queryByText('Isključite ruter 30 sekundi.')).not.toBeInTheDocument()
  })
})
