import { describe, it, expect, vi, beforeEach } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

// PB-56 / US-81: AttachmentList prikazuje slike kao thumbnails, dokumente sa download dugmetom,
// uz meta podatke (uploadedBy, uploadedAt). Sadržaj se uvijek fetcha preko autentifikovanog axiosa
// (Bearer token) — pa stub-amo attachmentService.

vi.mock('../services/attachmentService', () => ({
  fetchAttachmentBlobUrl: vi.fn(() => Promise.resolve('blob:fake-url')),
  downloadAttachment: vi.fn(() => Promise.resolve()),
}))

import AttachmentList from '../components/common/AttachmentList'
import { fetchAttachmentBlobUrl, downloadAttachment } from '../services/attachmentService'

describe('AttachmentList', () => {
  beforeEach(() => {
    fetchAttachmentBlobUrl.mockClear()
    downloadAttachment.mockClear()
  })

  it('renders nothing when attachments are empty', () => {
    const { container } = render(<AttachmentList attachments={[]} />)
    expect(container.firstChild).toBeNull()
  })

  it('renders nothing when attachments is null', () => {
    const { container } = render(<AttachmentList attachments={null} />)
    expect(container.firstChild).toBeNull()
  })

  it('renders image thumbnails for image attachments', async () => {
    const attachments = [
      {
        attachmentId: 1,
        fileName: 'slika.png',
        contentType: 'image/png',
        size: 1024,
        downloadUrl: '/api/attachments/1',
        uploadedAt: '2026-05-01T10:00:00Z',
        uploadedByName: 'Amina Begić',
      },
    ]
    render(<AttachmentList attachments={attachments} />)

    await waitFor(() => {
      expect(fetchAttachmentBlobUrl).toHaveBeenCalledWith(1)
    })
    const thumbs = await screen.findAllByTestId('attachment-thumbnail')
    expect(thumbs).toHaveLength(1)
    expect(thumbs[0]).toHaveAttribute('src', 'blob:fake-url')
    expect(thumbs[0]).toHaveAttribute('alt', 'slika.png')
  })

  it('renders document attachments as download buttons', () => {
    const attachments = [
      {
        attachmentId: 2,
        fileName: 'ugovor.pdf',
        contentType: 'application/pdf',
        size: 2048,
        downloadUrl: '/api/attachments/2',
        uploadedAt: '2026-05-01T10:00:00Z',
        uploadedByName: 'Agent Test',
      },
    ]
    render(<AttachmentList attachments={attachments} />)

    const docs = screen.getAllByTestId('attachment-doc')
    expect(docs).toHaveLength(1)
    expect(screen.getByText('ugovor.pdf')).toBeInTheDocument()
  })

  it('triggers authenticated download when document is clicked', async () => {
    const attachments = [
      {
        attachmentId: 5,
        fileName: 'ugovor.pdf',
        contentType: 'application/pdf',
        size: 2048,
        downloadUrl: '/api/attachments/5',
      },
    ]
    render(<AttachmentList attachments={attachments} />)

    fireEvent.click(screen.getByTestId('attachment-doc'))
    await waitFor(() => {
      expect(downloadAttachment).toHaveBeenCalledWith(5, 'ugovor.pdf')
    })
  })

  it('shows uploadedBy and uploadedAt in meta line', () => {
    const attachments = [
      {
        attachmentId: 3,
        fileName: 'note.txt',
        contentType: 'text/plain',
        size: 50,
        downloadUrl: '/api/attachments/3',
        uploadedAt: '2026-05-01T10:00:00Z',
        uploadedByName: 'Amina Begić',
      },
    ]
    render(<AttachmentList attachments={attachments} />)

    const meta = screen.getAllByTestId('attachment-meta')
    expect(meta.length).toBeGreaterThan(0)
    expect(meta[0].textContent).toContain('Amina Begić')
  })

  it('opens lightbox when image thumbnail is clicked', async () => {
    const attachments = [
      {
        attachmentId: 4,
        fileName: 'velika.jpg',
        contentType: 'image/jpeg',
        size: 1024,
        downloadUrl: '/api/attachments/4',
        uploadedAt: '2026-05-01T10:00:00Z',
        uploadedByName: 'Klijent',
      },
    ]
    render(<AttachmentList attachments={attachments} />)

    const thumbButton = await screen.findByLabelText('Otvori sliku velika.jpg')
    fireEvent.click(thumbButton)

    expect(screen.getByTestId('attachment-lightbox')).toBeInTheDocument()
  })
})
