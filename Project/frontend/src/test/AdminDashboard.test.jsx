import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAdminDashboard: vi.fn(),
  generateReport: vi.fn(),
}))

vi.mock('../services/adminService', () => ({
  getAdminDashboard: mocks.getAdminDashboard,
  generateReport: mocks.generateReport,
}))

vi.mock('recharts', () => ({
  ResponsiveContainer: ({ children }) => <div data-testid="chart">{children}</div>,
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

import AdminDashboardSection from '../components/admin/AdminDashboardSection'

const DASHBOARD_DATA = {
  period: { periodLabel: 'Mjesec' },
  totalTicketsInPeriod: 10,
  statusBreakdown: [
    { status: 'OPEN', count: 4, percentage: 40 },
    { status: 'CLOSED', count: 6, percentage: 60 },
  ],
  avgFirstResponseMinutes: 25,
  firstResponseBucketGranularityLabel: 'Po sedmici',
  firstResponseByPeriod: [
    { label: 'Sedmica 1', ticketCount: 3, ticketsWithResponseCount: 2, avgFirstResponseMinutes: 20 },
    { label: 'Sedmica 2', ticketCount: 2, ticketsWithResponseCount: 2, avgFirstResponseMinutes: 35 },
  ],
  avgResolutionHours: 2,
  avgRating: 4.5,
  topProblemTypes: [{ name: 'INTERNET', count: 5 }],
  topAgentWorkload: [{ userId: 2, fullName: 'Agent One', role: 'AGENT', resolvedCount: 3 }],
  activeUsersByRole: { clients: 5, agents: 2, technicians: 1, administrators: 1 },
  openTicketsCount: 4,
  closureRequestedCount: 1,
  unassignedOpenCount: 0,
  staleTicketsCount: 0,
}

function renderAdminDashboard() {
  return render(
    <MemoryRouter>
      <AdminDashboardSection mode="metrics" />
    </MemoryRouter>,
  )
}

describe('AdminDashboardSection (PB-45)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAdminDashboard.mockResolvedValue(DASHBOARD_DATA)
  })

  it('prikazuje KPI kartice nakon učitavanja', async () => {
    renderAdminDashboard()

    await waitFor(() => {
      expect(screen.getByText('Ključne metrike')).toBeInTheDocument()
    })
    expect(screen.getByText('10')).toBeInTheDocument()
    expect(screen.getByText('Kreirani tiketi')).toBeInTheDocument()
    expect(screen.getByText('Zatvoreni')).toBeInTheDocument()
    expect(screen.queryByText('Admin dashboard')).not.toBeInTheDocument()
    expect(screen.queryByText('Nedodijeljeni')).not.toBeInTheDocument()
  })

  it('ne prikazuje generisanje izvještaja u metrics modu', async () => {
    renderAdminDashboard()

    await waitFor(() => {
      expect(screen.getByText('Ključne metrike')).toBeInTheDocument()
    })
    expect(screen.queryByText('Generisanje izvještaja')).not.toBeInTheDocument()
    expect(screen.queryByRole('button', { name: /export/i })).not.toBeInTheDocument()
  })

  it('prikazuje KPI prosj. prvog odgovora bez grafa trenda', async () => {
    renderAdminDashboard()
    await waitFor(() => {
      expect(screen.getByText('Prosj. 1. odgovor')).toBeInTheDocument()
    })
    expect(screen.queryByText('Prosj. prvi odgovor')).not.toBeInTheDocument()
  })

  it('prikazuje vremenski filter', async () => {
    renderAdminDashboard()

    await waitFor(() => {
      expect(screen.getByRole('button', { name: 'Sedmica' })).toBeInTheDocument()
      expect(screen.getByRole('button', { name: 'Mjesec' })).toBeInTheDocument()
    })
  })
})
