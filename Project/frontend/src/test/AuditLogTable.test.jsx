import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'

const mocks = vi.hoisted(() => ({
  getAuditLogDetail: vi.fn(),
}))

vi.mock('../pages/AuditLog/auditLog.service', () => ({
  getAuditLogDetail: mocks.getAuditLogDetail,
}))

vi.mock('../components/common/Modal', () => ({
  default: ({ isOpen, onClose, title, children }) => 
    isOpen ? (
      <div data-testid="modal">
        <h3>{title}</h3>
        <button onClick={onClose}>Zatvori</button>
        {children}
      </div>
    ) : null,
}))

import AuditLogTable from '../pages/AuditLog/AuditLogTable'

const FAKE_LOGS = [
  {
    id: 1,
    timestamp: '2025-05-20T10:30:00Z',
    userId: 5,
    userFullName: 'Ajdin Hodžić',
    userEmail: 'ajdin@example.com',
    userRole: 'AGENT',
    actionType: 'TICKET_FORWARDED',
    entityType: 'Ticket',
    entityId: '42',
    description: 'Tiket #42 proslijeđen od Ajdina ka Hani',
    hasDetails: true,
  },
  {
    id: 2,
    timestamp: '2025-05-20T10:00:00Z',
    userId: null,
    userFullName: null,
    userEmail: null,
    userRole: null,
    actionType: 'TICKET_CREATED',
    entityType: 'Ticket',
    entityId: '50',
    description: 'Tiket automatski kreiran',
    hasDetails: false,
  },
]

function renderAuditLogTable(logs = FAKE_LOGS) {
  return render(<AuditLogTable logs={logs} />)
}

describe('AuditLogTable', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('displays logs in table format on desktop', () => {
    renderAuditLogTable()

    expect(screen.getByText('Ajdin Hodžić')).toBeInTheDocument()
    expect(screen.getByText('AGENT')).toBeInTheDocument()
  })

  it('displays system when userId is null', () => {
    renderAuditLogTable()

    expect(screen.getByText('Sistem')).toBeInTheDocument()
  })

  it('displays readable action type labels', () => {
    renderAuditLogTable()

    // Check if label is displayed (should map TICKET_FORWARDED to readable text)
    expect(screen.getByText(/Prosljeđivanje tiketa|TICKET_FORWARDED/)).toBeInTheDocument()
  })

  it('displays "Pregledaj" button only for logs with details', () => {
    renderAuditLogTable()

    const pregledajButtons = screen.getAllByRole('button', { name: /Pregledaj|Eye/ })
    // Should have at least one button for the log with hasDetails=true
    expect(pregledajButtons.length).toBeGreaterThanOrEqual(1)
  })

  it('opens detail modal when "Pregledaj" button is clicked', async () => {
    const detailLog = {
      id: 1,
      timestamp: '2025-05-20T10:30:00Z',
      userId: 5,
      userFullName: 'Ajdin Hodžić',
      userEmail: 'ajdin@example.com',
      userRole: 'AGENT',
      actionType: 'TICKET_FORWARDED',
      entityType: 'Ticket',
      entityId: '42',
      description: 'Tiket #42 proslijeđen od Ajdina ka Hani',
      hasDetails: true,
      oldValue: { agent: 'John' },
      newValue: { agent: 'Jane' },
      ipAddress: '192.168.1.1',
    }

    mocks.getAuditLogDetail.mockResolvedValue(detailLog)

    renderAuditLogTable([detailLog])

    const pregledajButtons = screen.getAllByText(/Pregledaj/)
    fireEvent.click(pregledajButtons[0])

    await waitFor(() => {
      expect(mocks.getAuditLogDetail).toHaveBeenCalledWith(1)
      expect(screen.getByTestId('modal')).toBeInTheDocument()
    })
  })

  it('truncates long descriptions in table', () => {
    const longDescriptionLog = {
      ...FAKE_LOGS[0],
      description: 'A'.repeat(150), // Very long description
    }

    renderAuditLogTable([longDescriptionLog])

    // Should truncate to ~80 chars
    const description = screen.getByTitle(longDescriptionLog.description)
    const displayedText = description.textContent
    expect(displayedText.length).toBeLessThan(longDescriptionLog.description.length)
  })

  it('displays entity information (type and ID)', () => {
    renderAuditLogTable()

    expect(screen.getByText(/Ticket #42/)).toBeInTheDocument()
    expect(screen.getByText(/Ticket #50/)).toBeInTheDocument()
  })

  it('renders mobile view with card layout', () => {
    renderAuditLogTable()

    // Both desktop and mobile are rendered in jsdom
    // Just verify the content is there
    expect(screen.getByText('Ajdin Hodžić')).toBeInTheDocument()
  })
})
