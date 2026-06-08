// PB-45 / US-71, US-72, US-82, US-86 + PB-50 / US-87 — Frontend testovi
import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, fireEvent, waitFor, act } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const mocks = vi.hoisted(() => ({
  getAdminDashboard: vi.fn(),
  generateReport: vi.fn(),
  navigate: vi.fn(),
}))

vi.mock('../services/adminService', () => ({
  getAdminDashboard: mocks.getAdminDashboard,
  generateReport: mocks.generateReport,
  getSlaBreachCount: vi.fn().mockResolvedValue(0),
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

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => mocks.navigate,
  }
})

import AdminDashboardSection from '../components/admin/AdminDashboardSection'

const FULL_DASHBOARD = {
  period: { periodLabel: 'Mjesec' },
  totalTicketsInPeriod: 10,
  statusBreakdown: [
    { status: 'OPEN', count: 4, percentage: 40 },
    { status: 'CLOSED', count: 5, percentage: 50 },
    { status: 'CLOSURE_REQUESTED', count: 1, percentage: 10 },
  ],
  avgFirstResponseMinutes: 25,
  firstResponseBucketGranularityLabel: 'Po sedmici',
  firstResponseByPeriod: [],
  avgResolutionHours: 2,
  closedInPeriodCount: 5,
  avgRating: 4.5,
  topProblemTypes: [{ name: 'INTERNET', count: 5 }],
  topAgentWorkload: [{ userId: 2, fullName: 'Agent One', role: 'AGENT', resolvedCount: 3 }],
  activeUsersByRole: { clients: 5, agents: 2, technicians: 1, administrators: 1 },
  openTicketsCount: 4,
  closureRequestedCount: 1,
  unassignedOpenCount: 0,
  staleTicketsCount: 0,
}

const EMPTY_DASHBOARD = {
  period: { periodLabel: 'Mjesec' },
  totalTicketsInPeriod: 0,
  statusBreakdown: [],
  avgFirstResponseMinutes: null,
  firstResponseBucketGranularityLabel: '',
  firstResponseByPeriod: [],
  avgResolutionHours: null,
  closedInPeriodCount: 0,
  avgRating: null,
  topProblemTypes: [],
  topAgentWorkload: [],
  activeUsersByRole: { clients: 0, agents: 0, technicians: 0, administrators: 0 },
  openTicketsCount: 0,
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

describe('AdminDashboardSection (metrics) — KPI i grafovi (PB-45 / US-71, US-82, US-86)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAdminDashboard.mockResolvedValue(FULL_DASHBOARD)
  })

  it('prikazuje sve must-have KPI kartice', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Ključne metrike')).toBeInTheDocument())
    expect(screen.getByText('Kreirani tiketi')).toBeInTheDocument()
    expect(screen.getByText('Prosj. 1. odgovor')).toBeInTheDocument()
    expect(screen.getByText('Prosj. rješavanje')).toBeInTheDocument()
    expect(screen.getByText('Prosj. ocjena')).toBeInTheDocument()
    expect(screen.getByText('Otvoreni (trenutno)')).toBeInTheDocument()
    expect(screen.getByText('Čeka zatvaranje')).toBeInTheDocument()
    expect(screen.getByText('Zatvoreni')).toBeInTheDocument()
    expect(screen.getByText('Zastarjeli (7+ dana)')).toBeInTheDocument()
  })

  it('prikazuje aktivne korisnike po rolama', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Aktivni korisnici')).toBeInTheDocument())
    expect(screen.getByText('Klijenti')).toBeInTheDocument()
    expect(screen.getByText('Agenti')).toBeInTheDocument()
    expect(screen.getByText('Tehničari')).toBeInTheDocument()
    expect(screen.getByText('Admini')).toBeInTheDocument()
  })

  it('prikazuje grafove na metrics modu', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Analiza tiketa')).toBeInTheDocument())
    expect(screen.getByText('Po statusu')).toBeInTheDocument()
    expect(screen.getByText('Top tipovi problema')).toBeInTheDocument()
    expect(screen.getByText('Opterećenje agenata')).toBeInTheDocument()
  })

  it('NE prikazuje generisanje izvještaja u metrics modu', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Ključne metrike')).toBeInTheDocument())
    expect(screen.queryByRole('button', { name: /^Export$/i })).not.toBeInTheDocument()
  })
})

describe('AdminDashboardSection (metrics) — prazno stanje (PB-45 / US-71)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAdminDashboard.mockResolvedValue(EMPTY_DASHBOARD)
  })

  it('grafovi prikazuju poruku umjesto praznog grafikona', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Analiza tiketa')).toBeInTheDocument())
    expect(screen.getAllByText(/Nema podataka/i).length).toBeGreaterThan(0)
  })

  it('Prosj. 1. odgovor KPI prikazuje empty poruku kada nema odgovora', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Nema odgovora u periodu')).toBeInTheDocument())
  })
})

describe('Globalni vremenski filter (PB-45 / US-72)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAdminDashboard.mockResolvedValue(FULL_DASHBOARD)
  })

  it('prikazuje brze periode (Dan / Sedmica / Mjesec / Godina) i prilagođeno', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByRole('button', { name: 'Dan' })).toBeInTheDocument())
    expect(screen.getByRole('button', { name: 'Sedmica' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Mjesec' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Godina' })).toBeInTheDocument()
    expect(screen.getByRole('button', { name: 'Prilagođeno' })).toBeInTheDocument()
  })

  it('promjena perioda i Primijeni dugme osvježava dashboard', async () => {
    renderMetrics()
    await waitFor(() => expect(mocks.getAdminDashboard).toHaveBeenCalled())
    mocks.getAdminDashboard.mockClear()

    await act(async () => {
      fireEvent.click(screen.getByRole('button', { name: 'Sedmica' }))
      fireEvent.click(screen.getByRole('button', { name: 'Primijeni' }))
    })

    await waitFor(() =>
      expect(mocks.getAdminDashboard).toHaveBeenCalledWith(expect.objectContaining({ period: 'week' })),
    )
  })

  it('nevalidan custom raspon prikazuje grešku na Primijeni i blokira Generiši izvještaj', async () => {
    mocks.generateReport.mockResolvedValue({ hasData: true, data: {} })
    renderMetrics()
    await waitFor(() => expect(mocks.getAdminDashboard).toHaveBeenCalled())

    await act(async () => {
      fireEvent.click(screen.getByRole('button', { name: 'Prilagođeno' }))
    })

    const dateInputs = document.querySelectorAll('input[type="date"]')
    expect(dateInputs.length).toBe(2)

    await act(async () => {
      fireEvent.change(dateInputs[0], { target: { value: '2026-12-31' } })
      fireEvent.change(dateInputs[1], { target: { value: '2026-01-01' } })
      fireEvent.click(screen.getByRole('button', { name: 'Primijeni' }))
    })

    await waitFor(() =>
      expect(screen.getByText(/Datum kraja mora biti nakon datuma početka/i)).toBeInTheDocument(),
    )
  })
})

describe('Drill-down (PB-45 / US-84)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.getAdminDashboard.mockResolvedValue(FULL_DASHBOARD)
  })

  it('klik na Kreirani tiketi KPI navigira na /tickets', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Kreirani tiketi')).toBeInTheDocument())

    fireEvent.click(screen.getByText('Kreirani tiketi'))

    await waitFor(() =>
      expect(mocks.navigate).toHaveBeenCalledWith(expect.stringMatching(/^\/tickets\?/)),
    )
  })

  it('klik na Otvoreni KPI navigira na /tickets sa status=OPEN', async () => {
    renderMetrics()
    await waitFor(() => expect(screen.getByText('Otvoreni (trenutno)')).toBeInTheDocument())

    fireEvent.click(screen.getByText('Otvoreni (trenutno)'))

    await waitFor(() => {
      const target = mocks.navigate.mock.calls.find(([url]) => url.includes('status=OPEN'))
      expect(target).toBeTruthy()
    })
  })
})

describe('AdminDashboardSection (reports mode) — generisanje + export (PB-45 / US-83, US-85)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mocks.generateReport.mockResolvedValue({
      hasData: true,
      reportType: 'TICKET_COUNT',
      data: { totalCount: 7 },
      showLargePeriodWarning: false,
    })
    URL.createObjectURL = vi.fn(() => 'blob:mock')
    URL.revokeObjectURL = vi.fn()
  })

  it('reports mod NE prikazuje KPI kartice i NE poziva dashboard endpoint', async () => {
    renderReports()
    expect(screen.getByRole('button', { name: 'Sedmica' })).toBeInTheDocument()
    expect(screen.getByText('Broj tiketa')).toBeInTheDocument()
    expect(mocks.getAdminDashboard).not.toHaveBeenCalled()
    expect(screen.queryByText('Ključne metrike')).not.toBeInTheDocument()
  })

  it('podržani svi report tipovi prikazani kao chip dugmad', () => {
    renderReports()
    const expectedLabels = [
      'Broj tiketa', 'Status tiketa', 'Tip problema',
      'Opterećenje tima', 'Ocjene korisnika', 'Prosj. prvi odgovor',
    ]
    expectedLabels.forEach(lbl =>
      expect(screen.getByRole('button', { name: lbl })).toBeInTheDocument(),
    )
  })

  it('Export dugme je aktivno i pokreće preuzimanje CSV-a (US-85)', async () => {
    renderReports()
    const exportBtn = screen.getByRole('button', { name: /Export/i })
    expect(exportBtn).not.toBeDisabled()
    fireEvent.click(exportBtn)
    await waitFor(() => expect(mocks.generateReport).toHaveBeenCalled())
  })

  it('klik na chip poziva generateReport sa odabranim tipom', async () => {
    renderReports()
    fireEvent.click(screen.getByRole('button', { name: 'Broj tiketa' }))

    await waitFor(() => expect(mocks.generateReport).toHaveBeenCalled())
    expect(mocks.generateReport).toHaveBeenCalledWith(expect.objectContaining({
      reportType: 'TICKET_COUNT',
      period: 'week',
    }))
  })

  it('warning za veliki period se prikazuje na TICKET_STATUS', async () => {
    mocks.generateReport.mockResolvedValue({
      hasData: true,
      reportType: 'TICKET_STATUS',
      data: { items: [] },
      showLargePeriodWarning: true,
    })
    renderReports()
    fireEvent.click(screen.getByRole('button', { name: 'Status tiketa' }))

    await waitFor(() => expect(screen.getByText(/Upozorenje/i)).toBeInTheDocument())
  })

  it('prazan rezultat prikazuje informativnu poruku', async () => {
    mocks.generateReport.mockResolvedValue({ hasData: false, message: 'Nema podataka za odabrani period.' })
    renderReports()
    fireEvent.click(screen.getByRole('button', { name: 'Broj tiketa' }))
    await waitFor(() => expect(screen.getByText('Nema podataka za odabrani period.')).toBeInTheDocument())
  })
})
