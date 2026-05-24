import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAdminDashboard: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../services/adminService', () => ({
  getAdminDashboard: mocks.getAdminDashboard,
  generateReport: vi.fn().mockResolvedValue({ hasData: true, data: {} }),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

vi.mock('recharts', () => ({
  ResponsiveContainer: ({ children }) => <div>{children}</div>,
  PieChart: ({ children }) => <div>{children}</div>,
  Pie: () => null,
  Cell: () => null,
  Tooltip: () => null,
  Legend: () => null,
  BarChart: ({ children }) => <div>{children}</div>,
  Bar: () => null,
  XAxis: () => null,
  YAxis: () => null,
  CartesianGrid: () => null,
}))

import Reports from '../pages/Reports'

const ADMIN = { role: 'ADMINISTRATOR', firstName: 'Admin' }

const DASHBOARD_DATA = {
  period: { periodLabel: 'Mjesec' },
  totalTicketsInPeriod: 3,
  statusBreakdown: [{ status: 'OPEN', count: 3, percentage: 100 }],
  avgFirstResponseMinutes: 20,
  avgResolutionHours: 1,
  avgRating: 4,
  topProblemTypes: [],
  topAgentWorkload: [],
  activeUsersByRole: { clients: 1, agents: 1, technicians: 0, administrators: 1 },
  openTicketsCount: 3,
  closureRequestedCount: 0,
  unassignedOpenCount: 0,
  staleTicketsCount: 0,
  firstResponseBucketGranularityLabel: 'Po sedmici',
  firstResponseByPeriod: [],
}

describe('Reports page (/reports)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.useAuth.mockReturnValue({ user: ADMIN })
    mocks.getAdminDashboard.mockResolvedValue(DASHBOARD_DATA)
  })

  it('prikazuje naslov stranice Izvještaji', async () => {
    render(
      <MemoryRouter>
        <Reports />
      </MemoryRouter>,
    )

    await waitFor(() => {
      expect(screen.getByRole('heading', { name: 'Izvještaji', level: 2 })).toBeInTheDocument()
    })
    expect(screen.getByText(/generisanje izvještaja za odabrani vremenski period/i)).toBeInTheDocument()
  })

  it('prikazuje disabled Export dugme', async () => {
    render(
      <MemoryRouter>
        <Reports />
      </MemoryRouter>,
    )

    await waitFor(() => expect(screen.getByText('Export')).toBeInTheDocument())
    expect(screen.getByRole('button', { name: /export/i })).toBeDisabled()
  })

  it('prikazuje vremenski period i generisanje izvještaja', async () => {
    render(
      <MemoryRouter>
        <Reports />
      </MemoryRouter>,
    )

    await waitFor(() => {
      expect(screen.getByText('Vremenski period')).toBeInTheDocument()
      expect(screen.getByText('Broj tiketa')).toBeInTheDocument()
    })
    expect(mocks.getAdminDashboard).not.toHaveBeenCalled()
    expect(screen.queryByText('Ključne metrike')).not.toBeInTheDocument()
  })
})
