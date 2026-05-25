import { describe, it, expect, beforeEach, vi } from 'vitest'

const mocks = vi.hoisted(() => ({
  apiGet: vi.fn(),
}))

vi.mock('../services/api', () => ({
  default: {
    get: mocks.apiGet,
  },
}))

import { getAuditLogs, getAuditLogDetail, getAuditActionTypes, getAuditLogUsers } from '../pages/AuditLog/auditLog.service'

const FAKE_RESPONSE = {
  items: [
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
      description: 'Tiket #42 proslijeđen',
      hasDetails: true,
    },
  ],
  totalCount: 100,
  page: 1,
  pageSize: 20,
  totalPages: 5,
}

const FAKE_DETAIL = {
  ...FAKE_RESPONSE.items[0],
  oldValue: { agent: 'John' },
  newValue: { agent: 'Jane' },
  ipAddress: '192.168.1.1',
}

describe('auditLog.service', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('getAuditLogs calls GET /audit-logs with pagination params', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_RESPONSE })

    await getAuditLogs(1, 20)

    expect(mocks.apiGet).toHaveBeenCalledWith('/audit-logs', {
      params: { page: 1, pageSize: 20 },
    })
  })

  it('getAuditLogs includes filter params when provided', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_RESPONSE })

    await getAuditLogs(1, 20, {
      search: 'forwarded',
      actionType: 'TICKET_FORWARDED',
      userId: '5',
      dateFrom: '2025-05-01',
      dateTo: '2025-05-31',
    })

    expect(mocks.apiGet).toHaveBeenCalledWith('/audit-logs', {
      params: expect.objectContaining({
        page: 1,
        pageSize: 20,
        search: 'forwarded',
        actionType: 'TICKET_FORWARDED',
        userId: '5',
        dateFrom: '2025-05-01',
        dateTo: '2025-05-31',
      }),
    })
  })

  it('getAuditLogs returns AuditLogResponse', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_RESPONSE })

    const result = await getAuditLogs(1, 20)

    expect(result).toEqual(FAKE_RESPONSE)
    expect(result.items).toHaveLength(1)
    expect(result.totalCount).toBe(100)
    expect(result.totalPages).toBe(5)
  })

  it('getAuditLogDetail calls GET /audit-logs/{id}', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_DETAIL })

    await getAuditLogDetail(1)

    expect(mocks.apiGet).toHaveBeenCalledWith('/audit-logs/1')
  })

  it('getAuditLogDetail returns deserialized oldValue and newValue', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_DETAIL })

    const result = await getAuditLogDetail(1)

    expect(result.oldValue).toEqual({ agent: 'John' })
    expect(result.newValue).toEqual({ agent: 'Jane' })
    expect(result.ipAddress).toBe('192.168.1.1')
  })

  it('getAuditActionTypes calls GET /audit-logs/action-types', async () => {
    const actionTypes = ['USER_LOGIN', 'TICKET_CREATED', 'TICKET_CLOSED']
    mocks.apiGet.mockResolvedValue({ data: actionTypes })

    await getAuditActionTypes()

    expect(mocks.apiGet).toHaveBeenCalledWith('/audit-logs/action-types')
  })

  it('getAuditActionTypes returns array of action type strings', async () => {
    const actionTypes = ['USER_LOGIN', 'TICKET_CREATED', 'TICKET_CLOSED']
    mocks.apiGet.mockResolvedValue({ data: actionTypes })

    const result = await getAuditActionTypes()

    expect(Array.isArray(result)).toBe(true)
    expect(result).toEqual(actionTypes)
  })

  it('getAuditLogUsers calls GET /audit-logs/users', async () => {
    const users = [
      { id: 5, fullName: 'Ajdin Hodžić', email: 'ajdin@example.com' },
    ]
    mocks.apiGet.mockResolvedValue({ data: users })

    await getAuditLogUsers()

    expect(mocks.apiGet).toHaveBeenCalledWith('/audit-logs/users')
  })

  it('getAuditLogUsers returns array of AuditLogUserDto', async () => {
    const users = [
      { id: 5, fullName: 'Ajdin Hodžić', email: 'ajdin@example.com' },
      { id: 1, fullName: 'Admin', email: 'admin@example.com' },
    ]
    mocks.apiGet.mockResolvedValue({ data: users })

    const result = await getAuditLogUsers()

    expect(Array.isArray(result)).toBe(true)
    expect(result).toHaveLength(2)
    expect(result[0]).toHaveProperty('id')
    expect(result[0]).toHaveProperty('fullName')
    expect(result[0]).toHaveProperty('email')
  })

  it('getAuditLogs excludes undefined filter params', async () => {
    mocks.apiGet.mockResolvedValue({ data: FAKE_RESPONSE })

    await getAuditLogs(1, 20, {
      search: 'test',
      actionType: undefined,
      userId: undefined,
    })

    const callParams = mocks.apiGet.mock.calls[0][1].params
    expect(callParams).toHaveProperty('search')
    expect(callParams).not.toHaveProperty('actionType')
    expect(callParams).not.toHaveProperty('userId')
  })
})
