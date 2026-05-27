import { describe, it, expect, beforeEach, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'

const state = vi.hoisted(() => ({ role: 'ADMINISTRATOR' }))

vi.mock('../context/AuthContext', () => ({
  useAuth: () => ({ user: { firstName: 'A', lastName: 'B', role: state.role } }),
}))

vi.mock('../context/NotificationContext', () => ({
  useNotifications: () => ({
    notifications: [],
    unreadCount: 0,
    markAsRead: vi.fn(),
    markAllAsRead: vi.fn(),
  }),
}))

vi.mock('../store/uiStore', () => ({
  useUIStore: () => ({
    aiPanelOpen: false,
    toggleAiPanel: vi.fn(),
    adminCopilotOpen: false,
    toggleAdminCopilot: vi.fn(),
  }),
}))

import Header from '../components/layout/Header'

function renderHeader() {
  return render(
    <MemoryRouter>
      <Header onMenuToggle={vi.fn()} title="Dashboard" />
    </MemoryRouter>
  )
}

describe('Header — MCP Copilot dugme (PB-70 / US-108)', () => {
  beforeEach(() => {
    state.role = 'ADMINISTRATOR'
  })

  it('administrator vidi MCP Copilot dugme', () => {
    state.role = 'ADMINISTRATOR'
    renderHeader()
    expect(screen.getByText('MCP Copilot')).toBeInTheDocument()
  })

  it('klijent NE vidi MCP Copilot dugme', () => {
    state.role = 'CLIENT'
    renderHeader()
    expect(screen.queryByText('MCP Copilot')).not.toBeInTheDocument()
  })
})
