import { describe, it, expect, beforeEach, vi } from 'vitest'

vi.mock('../services/api', () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}))

import api from '../services/api'
import {
  createFaq,
  deleteFaq,
  getAllFaqs,
  getFaqs,
  updateFaq,
} from '../services/faqService'

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

  // PB-61 / US-104: Admin endpointi

  it('getAllFaqs() calls GET /faq/all (admin endpoint)', async () => {
    api.get.mockResolvedValueOnce({ data: [] })

    await getAllFaqs()

    expect(api.get).toHaveBeenCalledWith('/faq/all')
  })

  it('createFaq() POSTs payload to /faq', async () => {
    const payload = { question: 'Q?', answer: 'A.', category: 'Internet', sortOrder: 1 }
    api.post.mockResolvedValueOnce({ data: { faqId: 5, ...payload } })

    const result = await createFaq(payload)

    expect(api.post).toHaveBeenCalledWith('/faq', payload)
    expect(result.faqId).toBe(5)
  })

  it('updateFaq() PUTs payload to /faq/:id', async () => {
    const payload = { question: 'Q2?', answer: 'A2.', category: null, sortOrder: 2 }
    api.put.mockResolvedValueOnce({ data: { faqId: 9, ...payload } })

    await updateFaq(9, payload)

    expect(api.put).toHaveBeenCalledWith('/faq/9', payload)
  })

  it('deleteFaq() DELETEs /faq/:id', async () => {
    api.delete.mockResolvedValueOnce({ data: undefined })

    await deleteFaq(11)

    expect(api.delete).toHaveBeenCalledWith('/faq/11')
  })
})
