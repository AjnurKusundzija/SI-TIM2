import { create } from 'zustand'

export const useUIStore = create((set) => ({
  aiPanelOpen: false,
  toggleAiPanel: () => set((s) => ({ aiPanelOpen: !s.aiPanelOpen })),
  closeAiPanel: () => set({ aiPanelOpen: false }),

  // tickets needing attention — set by AdminDashboardSection on dashboard load
  alertTicketCount: 0,
  alertTicketUrl: '',
  setAlert: (count, url) => set({ alertTicketCount: count, alertTicketUrl: url }),
}))
