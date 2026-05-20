import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'

// ─── Hoist mocks before any imports ────────────────────────────────────────
const mocks = vi.hoisted(() => ({
  getMyStatistics: vi.fn(),
  useAuth: vi.fn(),
}))

vi.mock('../services/userService', () => ({
  getMyStatistics: mocks.getMyStatistics,
  getMyRecentTickets: vi.fn().mockResolvedValue([]),
}))

vi.mock('../context/AuthContext', () => ({
  useAuth: mocks.useAuth,
}))

// Mock recharts to avoid canvas rendering issues in jsdom
vi.mock('recharts', () => ({
  ResponsiveContainer: ({ children }) => <div data-testid="chart-container">{children}</div>,
  PieChart:    ({ children }) => <div>{children}</div>,
  Pie:         () => null,
  Cell:        () => null,
  Tooltip:     () => null,
  Legend:      () => null,
  BarChart:    ({ children }) => <div>{children}</div>,
  Bar:         () => null,
  XAxis:       () => null,
  YAxis:       () => null,
  CartesianGrid: () => null,
}))

import Statistics from '../pages/Statistics'

// ─── Test data ──────────────────────────────────────────────────────────────
const AGENT_USER  = { role: 'AGENT',     firstName: 'Selma' }
const TECH_USER   = { role: 'TECHNICIAN', firstName: 'Amer' }

const FULL_STATS = {
  openTicketsCount:        5,
  closedTicketsCount:      10,
  pendingClosureCount:     2,
  avgFirstResponseMinutes: 30,
  avgResolutionHours:      2.5,
  avgRating:               4.2,
}

const EMPTY_STATS = {
  openTicketsCount:        0,
  closedTicketsCount:      0,
  pendingClosureCount:     0,
  avgFirstResponseMinutes: null,
  avgResolutionHours:      null,
  avgRating:               null,
}

function renderStatistics(user = AGENT_USER) {
  mocks.useAuth.mockReturnValue({ user })
  return render(<Statistics />)
}

describe('ReportsDashboard — Statistics page (US-41–US-48)', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  // US-41: Stats cards render with correct values
  it('shows open ticket count stat card', async () => {
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Otvoreni tiketi')).toBeInTheDocument()
    )
    expect(screen.getByText('5')).toBeInTheDocument()
  })

  it('shows closed ticket count stat card', async () => {
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Zatvoreni tiketi')).toBeInTheDocument()
    )
    expect(screen.getByText('10')).toBeInTheDocument()
  })

  it('shows pending closure count stat card', async () => {
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Čeka se')).toBeInTheDocument()
    )
    expect(screen.getByText('2')).toBeInTheDocument()
  })

  // US-44: Average resolution time displayed
  it('shows average resolution time card', async () => {
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Prosj. rješavanje')).toBeInTheDocument()
    )
  })

  // US-45: Average rating shown for AGENT role
  it('shows average rating card for AGENT role', async () => {
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics(AGENT_USER)

    await waitFor(() =>
      expect(screen.getByText('Prosječna ocjena')).toBeInTheDocument()
    )
  })

  // US-45: Average rating NOT shown for TECHNICIAN role
  it('does not show average rating for TECHNICIAN role', async () => {
    const techStats = { ...FULL_STATS, avgRating: null }
    mocks.getMyStatistics.mockResolvedValueOnce(techStats)
    renderStatistics(TECH_USER)

    await waitFor(() =>
      expect(screen.getByText('Otvoreni tiketi')).toBeInTheDocument()
    )
    expect(screen.queryByText('Prosječna ocjena')).not.toBeInTheDocument()
  })

  // US-41: Empty state — null metric values render "—" placeholder
  // (requires at least one ticket so the stats section renders instead of empty state)
  it('renders "—" placeholder for null metric values', async () => {
    const statsWithNullMetrics = {
      openTicketsCount:        3,   // non-zero → stats section renders
      closedTicketsCount:      0,
      pendingClosureCount:     0,
      avgFirstResponseMinutes: null,
      avgResolutionHours:      null,
      avgRating:               null,
    }
    mocks.getMyStatistics.mockResolvedValueOnce(statsWithNullMetrics)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Otvoreni tiketi')).toBeInTheDocument()
    )
    // "—" character shown for null avg values
    const dashes = screen.getAllByText('—')
    expect(dashes.length).toBeGreaterThan(0)
  })

  // Error state
  it('shows error message when statistics API fails', async () => {
    mocks.getMyStatistics.mockRejectedValueOnce(new Error('Network error'))
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText(/greška pri učitavanju statistike/i)).toBeInTheDocument()
    )
  })

  // Loading state
  it('shows loading indicator while statistics are loading', () => {
    mocks.getMyStatistics.mockReturnValueOnce(new Promise(() => {}))  // never resolves
    renderStatistics()

    expect(screen.getByText(/učitavanje statistike/i)).toBeInTheDocument()
  })

  // No console.error on render
  it('renders without console errors when data loads successfully', async () => {
    const consoleSpy = vi.spyOn(console, 'error').mockImplementation(() => {})
    mocks.getMyStatistics.mockResolvedValueOnce(FULL_STATS)
    renderStatistics()

    await waitFor(() =>
      expect(screen.getByText('Otvoreni tiketi')).toBeInTheDocument()
    )
    expect(consoleSpy).not.toHaveBeenCalled()
    consoleSpy.mockRestore()
  })
})
