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
import { adminCopilotQuery } from '../services/aiService'

describe('aiService.adminCopilotQuery (PB-70)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('POST-a pitanje na /ai/admin-copilot/query', async () => {
    api.post.mockResolvedValueOnce({ data: { answer: 'ok', intent: 'team_workload' } })

    await adminCopilotQuery('Koji tim je najopterećeniji?')

    expect(api.post).toHaveBeenCalledWith('/ai/admin-copilot/query', {
      question: 'Koji tim je najopterećeniji?',
      periodFrom: null,
      periodTo: null,
      dashboardContext: null,
    })
  })

  it('prosljeđuje dashboard kontekst', async () => {
    api.post.mockResolvedValueOnce({ data: {} })

    await adminCopilotQuery('test', { dashboardContext: 'period=week' })

    expect(api.post).toHaveBeenCalledWith(
      '/ai/admin-copilot/query',
      expect.objectContaining({ question: 'test', dashboardContext: 'period=week' })
    )
  })

  it('vraća podatke iz odgovora API-ja', async () => {
    const payload = { answer: 'Sažetak', intent: 'faq_coverage', usedTools: ['ticket.analytics'] }
    api.post.mockResolvedValueOnce({ data: payload })

    const result = await adminCopilotQuery('faq?')

    expect(result).toEqual(payload)
  })
})
