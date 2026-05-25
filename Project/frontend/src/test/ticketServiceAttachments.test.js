import { describe, it, expect, vi, beforeEach } from 'vitest'

// PB-56 / US-80: ticketService prosljeđuje onUploadProgress kroz axios opcije
// i mapira axios progress event u postotak (0–100).

vi.mock('../services/api', () => ({
  default: {
    post: vi.fn(),
  },
}))

import api from '../services/api'
import { createTicketWithAttachments, addCommentWithAttachments } from '../services/ticketService'

describe('ticketService attachments', () => {
  beforeEach(() => {
    api.post.mockReset()
    api.post.mockResolvedValue({ data: { ok: true } })
  })

  it('createTicketWithAttachments invokes axios with multipart and onUploadProgress', async () => {
    const file = new File([new Uint8Array(10)], 'a.png', { type: 'image/png' })
    const progressCb = vi.fn()

    await createTicketWithAttachments(
      { Subject: 'x', Type: 'INTERNET', Description: 'y', Priority: 'LOW' },
      [file],
      progressCb,
    )

    expect(api.post).toHaveBeenCalledTimes(1)
    const [url, , config] = api.post.mock.calls[0]
    expect(url).toBe('/tickets/attachments')
    expect(config.headers['Content-Type']).toBe('multipart/form-data')
    expect(typeof config.onUploadProgress).toBe('function')

    // simuliraj axios progress event
    config.onUploadProgress({ loaded: 50, total: 200 })
    expect(progressCb).toHaveBeenCalledWith(25)

    config.onUploadProgress({ loaded: 200, total: 200 })
    expect(progressCb).toHaveBeenCalledWith(100)
  })

  it('addCommentWithAttachments invokes axios with multipart and onUploadProgress', async () => {
    const file = new File([new Uint8Array(10)], 'b.txt', { type: 'text/plain' })
    const progressCb = vi.fn()

    await addCommentWithAttachments(42, 'hello', [file], progressCb)

    const [url, , config] = api.post.mock.calls[0]
    expect(url).toBe('/comment/tickets/42/attachments')
    expect(typeof config.onUploadProgress).toBe('function')

    config.onUploadProgress({ loaded: 10, total: 100 })
    expect(progressCb).toHaveBeenCalledWith(10)
  })

  it('does not throw when no progress callback is provided', async () => {
    const file = new File([new Uint8Array(10)], 'a.png', { type: 'image/png' })
    await createTicketWithAttachments(
      { Subject: 'x', Type: 'INTERNET', Description: 'y', Priority: 'LOW' },
      [file],
    )
    const [, , config] = api.post.mock.calls[0]
    // mora biti sigurno pozvati bez krasa
    expect(() => config.onUploadProgress({ loaded: 1, total: 1 })).not.toThrow()
  })
})
