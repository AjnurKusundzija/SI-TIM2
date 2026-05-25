// PB-50 / US-87, US-88 — Frontend test za prosječno vrijeme prvog odgovora
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
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

import AdminDashboardSection from '../components/admin/AdminDashboardSection'

const DASHBOARD_NO_REPLIES = {
  period: { periodLabel: 'Mjesec' },
  totalTicketsInPeriod: 5,
  statusBreakdown: [{ status: 'OPEN', count: 5, percentage: 100 }],
  avgFirstResponseMinutes: null, // US-87: nema lažne 0
  firstResponseBucketGranularityLabel: 'Po sedmici',
  firstResponseByPeriod: [],
  avgResolutionHours: null,
  closedInPeriodCount: 0,
  avgRating: null,
  topProblemTypes: [{ name: 'INTERNET', count: 5 }],
  topAgentWorkload: [],
  activeUsersByRole: { clients: 1, agents: 1, technicians: 0, administrators: 1 },
  openTicketsCount: 5,
  closureRequestedCount: 0,
  unassignedOpenCount: 0,
  staleTicketsCount: 0,
}

function renderMetrics() {
  return render(
    <MemoryRouter>
      <AdminDashboardSection mode="metrics" />
    </MemoryRouter>,
  )
}

function renderReports() {
  return render(
    <MemoryRouter>
      <AdminDashboardSection mode="reports" />
    </MemoryRouter>,
  )
}

describe('US-87 — Prosj. 1. odgovor KPI', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('prikazuje empty poruku kada nema staff odgovora (ne 0)', async () => {
    mocks.getAdminDashboard.mockResolvedValue(DASHBOARD_NO_REPLIES)
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Prosj. 1. odgovor')).toBeInTheDocument())
    expect(screen.getByText('Nema odgovora u periodu')).toBeInTheDocument()
    // ne smijemo vidjeti "0 min" za ovaj KPI
    const kpiCard = screen.getByText('Prosj. 1. odgovor').closest('button')
    expect(kpiCard.textContent).not.toMatch(/^Prosj\. 1\. odgovor\s*0/)
  })

  it('prikazuje formatiranu vrijednost kada ima podataka', async () => {
    mocks.getAdminDashboard.mockResolvedValue({ ...DASHBOARD_NO_REPLIES, avgFirstResponseMinutes: 90 })
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Prosj. 1. odgovor')).toBeInTheDocument())
    // 90 minuta = "1 h 30 min" prema formatDuration
    const kpiCard = screen.getByText('Prosj. 1. odgovor').closest('button')
    expect(kpiCard.textContent).toMatch(/1\s*h\s*30\s*min/i)
  })
})

describe('US-88 — FIRST_RESPONSE izvještaj na /reports', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('generiše FIRST_RESPONSE i prikazuje prosjek + bucket tabelu', async () => {
    mocks.generateReport.mockResolvedValue({
      hasData: true,
      reportType: 'FIRST_RESPONSE',
      data: {
        avgFirstResponseMinutes: 35,
        totalTicketsCount: 4,
        ticketsWithResponseCount: 3,
        bucketGranularity: 'day',
        bucketGranularityLabel: 'Po danu',
        buckets: [
          { label: '01.02.2026', ticketCount: 2, ticketsWithResponseCount: 2, avgFirstResponseMinutes: 20 },
          { label: '02.02.2026', ticketCount: 2, ticketsWithResponseCount: 1, avgFirstResponseMinutes: 50 },
        ],
      },
    })

    renderReports()
    fireEvent.click(screen.getByRole('button', { name: 'Prosj. prvi odgovor' }))

    await waitFor(() => expect(screen.getByText('Po danu')).toBeInTheDocument())
    expect(screen.getByText('S odgovorom / ukupno')).toBeInTheDocument()
    expect(screen.getByText('01.02.2026')).toBeInTheDocument()
    expect(screen.getByText('02.02.2026')).toBeInTheDocument()
  })

  it('FIRST_RESPONSE bez odgovora prikazuje informativnu poruku', async () => {
    mocks.generateReport.mockResolvedValue({
      hasData: true,
      reportType: 'FIRST_RESPONSE',
      message: 'Nema tiketa s prvim odgovorom u odabranom periodu.',
      data: {
        avgFirstResponseMinutes: null,
        totalTicketsCount: 5,
        ticketsWithResponseCount: 0,
        bucketGranularity: 'day',
        bucketGranularityLabel: 'Po danu',
        buckets: [],
      },
    })

    renderReports()
    fireEvent.click(screen.getByRole('button', { name: 'Prosj. prvi odgovor' }))

    await waitFor(() => expect(screen.getByText(/Nema tiketa s prvim odgovorom/i)).toBeInTheDocument())
    expect(screen.getByText(/Nema podataka po pod-periodima/i)).toBeInTheDocument()
  })
})
