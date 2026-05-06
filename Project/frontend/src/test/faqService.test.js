import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(),
  },
}))

import api from '../services/api'
import { getFaqs } from '../services/faqService'

describe('faqService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('getFaqs() calls GET /faq', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getFaqs()

    expect(api.get).toHaveBeenCalledWith('/faq')
  })

  it('getFaqs() returns data from API response', async () => {
    const faqs = [{ faqId: 1, question: 'Kako resetovati ruter?', answer: 'Restartujte ruter.' }]
    api.get.mockResolvedValueOnce({ data: faqs })

    const result = await getFaqs()

    expect(result).toEqual(faqs)
  })
})
