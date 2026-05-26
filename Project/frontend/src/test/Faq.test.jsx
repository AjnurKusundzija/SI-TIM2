import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  getFaqs: vi.fn(),
  getAllFaqs: vi.fn(),
  createFaq: vi.fn(),
  updateFaq: vi.fn(),
  deleteFaq: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../services/faqService', () => ({
  getFaqs: mocks.getFaqs,
  getAllFaqs: mocks.getAllFaqs,
  createFaq: mocks.createFaq,
  updateFaq: mocks.updateFaq,
  deleteFaq: mocks.deleteFaq,
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
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

describe('Faq page — read-only (CLIENT)', () => {
  let consoleErrorSpy

  beforeEach(() => {
    vi.clearAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'CLIENT' } })
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

  // PB-61 / US-104: ne-admin korisnici ne smiju vidjeti CRUD kontrole
  it('does not render admin CRUD controls for non-admin users', async () => {
    mocks.getFaqs.mockResolvedValueOnce(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    expect(screen.queryByRole('button', { name: /dodaj pitanje/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /uredi pitanje/i })).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /obriši pitanje/i })).not.toBeInTheDocument()
  })

  it('CLIENT uses public endpoint and not admin endpoint', async () => {
    mocks.getFaqs.mockResolvedValueOnce(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    expect(mocks.getFaqs).toHaveBeenCalled()
    expect(mocks.getAllFaqs).not.toHaveBeenCalled()
  })
})

// PB-61 / US-104: Admin CRUD nad FAQ stavkama
describe('Faq page — admin CRUD (ADMINISTRATOR)', () => {
  let consoleErrorSpy

  beforeEach(() => {
    // vi.resetAllMocks resetuje i mockResolvedValueOnce queue između testova
    vi.resetAllMocks()
    consoleErrorSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.useAuth.mockReturnValue({ user: { role: 'ADMINISTRATOR' } })
  })

  afterEach(() => {
    consoleErrorSpy.mockRestore()
  })

  it('uses admin endpoint and renders edit/delete controls for admin', async () => {
    mocks.getAllFaqs.mockResolvedValue(FAKE_FAQS)

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    expect(mocks.getAllFaqs).toHaveBeenCalled()
    expect(screen.getByRole('button', { name: /dodaj pitanje/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /uredi pitanje kako resetovati ruter/i })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /obriši pitanje kako resetovati ruter/i })).toBeInTheDocument()
  })

  // PB-61 / US-104: validacija praznog pitanja u admin formi
  it('shows validation error when question is empty', async () => {
    mocks.getAllFaqs.mockResolvedValue([])

    render(<Faq />)

    await waitFor(() => expect(screen.getByText(/Nema FAQ pitanja/i)).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /dodaj pitanje/i }))

    // Modal je otvoren; pronađi submit dugme (zadnje sa nazivom „Dodaj pitanje")
    const submitButtons = screen.getAllByRole('button', { name: /dodaj pitanje/i })
    fireEvent.click(submitButtons[submitButtons.length - 1])

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent(/pitanje ne smije biti prazno/i)
    )
    expect(mocks.createFaq).not.toHaveBeenCalled()
  })

  it('shows validation error when answer is empty', async () => {
    mocks.getAllFaqs.mockResolvedValue([])

    render(<Faq />)

    await waitFor(() => expect(screen.getByText(/Nema FAQ pitanja/i)).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /dodaj pitanje/i }))
    fireEvent.change(screen.getByLabelText('Pitanje', { selector: 'input' }), { target: { value: 'Validno pitanje?' } })

    const submitButtons = screen.getAllByRole('button', { name: /dodaj pitanje/i })
    fireEvent.click(submitButtons[submitButtons.length - 1])

    await waitFor(() =>
      expect(screen.getByRole('alert')).toHaveTextContent(/odgovor ne smije biti prazan/i)
    )
    expect(mocks.createFaq).not.toHaveBeenCalled()
  })

  it('creates a new FAQ when form is valid', async () => {
    mocks.getAllFaqs.mockResolvedValue([])
    mocks.createFaq.mockResolvedValue({})

    render(<Faq />)

    await waitFor(() => expect(screen.getByText(/Nema FAQ pitanja/i)).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /dodaj pitanje/i }))
    fireEvent.change(screen.getByLabelText('Pitanje', { selector: 'input' }), { target: { value: 'Novo pitanje?' } })
    fireEvent.change(screen.getByLabelText('Odgovor', { selector: 'textarea' }), { target: { value: 'Novi odgovor.' } })

    const submitButtons = screen.getAllByRole('button', { name: /dodaj pitanje/i })
    fireEvent.click(submitButtons[submitButtons.length - 1])

    await waitFor(() =>
      expect(mocks.createFaq).toHaveBeenCalledWith(
        expect.objectContaining({
          question: 'Novo pitanje?',
          answer: 'Novi odgovor.',
        })
      )
    )
  })

  it('opens edit form pre-filled and submits update', async () => {
    mocks.getAllFaqs.mockResolvedValue(FAKE_FAQS)
    mocks.updateFaq.mockResolvedValue({})

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    fireEvent.click(screen.getByRole('button', { name: /uredi pitanje kako resetovati ruter/i }))

    const questionInput = screen.getByLabelText('Pitanje', { selector: 'input' })
    expect(questionInput.value).toBe('Kako resetovati ruter?')

    fireEvent.change(questionInput, { target: { value: 'Izmijenjeno pitanje?' } })
    fireEvent.click(screen.getByRole('button', { name: /sačuvaj izmjene/i }))

    await waitFor(() =>
      expect(mocks.updateFaq).toHaveBeenCalledWith(
        1,
        expect.objectContaining({ question: 'Izmijenjeno pitanje?' })
      )
    )
  })

  it('confirms deletion before calling delete API and refreshes list', async () => {
    mocks.getAllFaqs.mockResolvedValue(FAKE_FAQS)
    mocks.deleteFaq.mockResolvedValue({})

    render(<Faq />)

    await waitFor(() => expect(screen.getByText('Kako resetovati ruter?')).toBeInTheDocument())

    const initialLoadCount = mocks.getAllFaqs.mock.calls.length

    fireEvent.click(screen.getByRole('button', { name: /obriši pitanje kako resetovati ruter/i }))

    // ConfirmDialog se otvori sa „Obriši" akcijom
    const confirmButtons = screen.getAllByRole('button', { name: /^obriši$/i })
    expect(confirmButtons.length).toBeGreaterThan(0)
    fireEvent.click(confirmButtons[confirmButtons.length - 1])

    await waitFor(() => expect(mocks.deleteFaq).toHaveBeenCalledWith(1))
    // nakon brisanja, lista se mora osvježiti (loadFaqs → getAllFaqs)
    await waitFor(() =>
      expect(mocks.getAllFaqs.mock.calls.length).toBeGreaterThan(initialLoadCount)
    )
  })
})
