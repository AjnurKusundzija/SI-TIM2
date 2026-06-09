import { create } from 'zustand'

export const useUIStore = create((set) => ({
  aiPanelOpen: false,
  toggleAiPanel: () => set((s) => ({ aiPanelOpen: !s.aiPanelOpen })),
  closeAiPanel: () => set({ aiPanelOpen: false }),

  // PB-70 / US-108: MCP Admin Copilot chat panel
  adminCopilotOpen: false,
  toggleAdminCopilot: () => set((s) => ({ adminCopilotOpen: !s.adminCopilotOpen })),
  closeAdminCopilot: () => set({ adminCopilotOpen: false }),

  // tickets needing attention — set by AdminDashboardSection on dashboard load
  alertTicketCount: 0,
  alertTicketUrl: '',
  setAlert: (count, url) => set({ alertTicketCount: count, alertTicketUrl: url }),
}))
