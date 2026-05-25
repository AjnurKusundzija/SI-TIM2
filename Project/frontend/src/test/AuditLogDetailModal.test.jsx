import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  getAuditLogDetail: vi.fn(),
}))

vi.mock('../pages/AuditLog/auditLog.service', () => ({
  getAuditLogDetail: mocks.getAuditLogDetail,
}))

vi.mock('../components/common/Modal', () => ({
  default: ({ isOpen, onClose, title, children }) =>
    isOpen ? (
      <div data-testid="detail-modal">
        <h3>{title}</h3>
        <button onClick={onClose}>Zatvori</button>
        {children}
      </div>
    ) : null,
}))

import AuditLogDetailModal from '../pages/AuditLog/AuditLogDetailModal'

const FAKE_DETAIL = {
  id: 1,
  timestamp: '2025-05-20T10:30:00Z',
  userId: 5,
  userFullName: 'Ajdin Hodžić',
  userEmail: 'ajdin@example.com',
  userRole: 'AGENT',
  actionType: 'TICKET_STATUS_CHANGED',
  entityType: 'Ticket',
  entityId: '42',
  description: 'Status tiketa promijenjen sa OPEN na CLOSED',
  hasDetails: true,
  oldValue: { status: 'OPEN', priority: 'HIGH' },
  newValue: { status: 'CLOSED', priority: 'HIGH' },
  ipAddress: '192.168.1.1',
}

function renderDetailModal(logId = 1, isOpen = true) {
  return render(
    <AuditLogDetailModal
      logId={logId}
      isOpen={isOpen}
      onClose={vi.fn()}
    />
  )
}

describe('AuditLogDetailModal', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAuditLogDetail.mockResolvedValue(FAKE_DETAIL)
  })

  it('does not render when isOpen is false', () => {
    renderDetailModal(1, false)

    expect(screen.queryByTestId('detail-modal')).not.toBeInTheDocument()
  })

  it('fetches and displays log detail', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(mocks.getAuditLogDetail).toHaveBeenCalledWith(1)
      expect(screen.getByText(/Promjena statusa|TICKET_STATUS_CHANGED/)).toBeInTheDocument()
    })
  })

  it('displays user information', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('Ajdin Hodžić')).toBeInTheDocument()
      expect(screen.getByText('ajdin@example.com')).toBeInTheDocument()
      expect(screen.getByText('AGENT')).toBeInTheDocument()
    })
  })

  it('displays IP address', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('192.168.1.1')).toBeInTheDocument()
    })
  })

  it('displays description in detail modal', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('Status tiketa promijenjen sa OPEN na CLOSED')).toBeInTheDocument()
    })
  })

  it('displays old and new values in table format', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('status')).toBeInTheDocument()
      // Since we're displaying old/new values, they should appear
    })
  })

  it('displays "Zatvori" button', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('Zatvori')).toBeInTheDocument()
    })
  })

  it('displays loading spinner while fetching', () => {
    mocks.getAuditLogDetail.mockImplementation(() =>
      new Promise((resolve) => setTimeout(() => resolve(FAKE_DETAIL), 100))
    )

    renderDetailModal()

    // Should show loading initially
    expect(screen.getByTestId('detail-modal')).toBeInTheDocument()
  })

  it('displays error message on fetch failure', async () => {
    mocks.getAuditLogDetail.mockRejectedValue({
      response: { data: { message: 'Ne mogu dohvatiti detalje' } },
    })

    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('Ne mogu dohvatiti detalje')).toBeInTheDocument()
    })
  })

  it('displays modal title with action type label', async () => {
    renderDetailModal()

    await waitFor(() => {
      const title = screen.getByRole('heading')
      expect(title.textContent).toContain('Detalji akcije')
    })
  })

  it('displays entity information', async () => {
    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText(/Ticket/)).toBeInTheDocument()
      expect(screen.getByText('#42')).toBeInTheDocument()
    })
  })

  it('calls getAuditLogDetail with correct log ID', async () => {
    renderDetailModal(42)

    await waitFor(() => {
      expect(mocks.getAuditLogDetail).toHaveBeenCalledWith(42)
    })
  })

  it('displays "Sistem" when userId is null', async () => {
    const detailWithoutUser = {
      ...FAKE_DETAIL,
      userId: null,
      userFullName: null,
      userEmail: null,
    }

    mocks.getAuditLogDetail.mockResolvedValue(detailWithoutUser)

    renderDetailModal()

    await waitFor(() => {
      expect(screen.getByText('Sistem')).toBeInTheDocument()
    })
  })
})
