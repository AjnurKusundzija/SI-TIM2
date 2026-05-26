import api from './api'

// PB-57 / US-96: Agent requests AI-suggested reply for a ticket
export async function getAgentSuggestion({ ticketId, title, description, category, recentComments }) {
  const response = await api.post('/ai/agent-suggestion', {
    ticketId,
    title,
    description,
    category,
    recentComments,
  })
  return response.data
}

// PB-58 / US-98, US-99: Admin requests AI insights based on dashboard metrics
export async function getAdminInsights(dashboardData) {
  const payload = {
    period: dashboardData.period?.periodLabel ?? dashboardData.period ?? 'Tekući period',
    totalTickets: dashboardData.totalTicketsInPeriod ?? 0,
    openTickets: dashboardData.openTicketsCount ?? 0,
    closedTickets: dashboardData.closedInPeriodCount ?? 0,
    closureRequestedTickets: dashboardData.closureRequestedCount ?? 0,
    unassignedTickets: dashboardData.unassignedOpenCount ?? 0,
    staleTickets: dashboardData.staleTicketsCount ?? 0,
    avgFirstResponseMinutes: dashboardData.avgFirstResponseMinutes ?? null,
    avgResolutionHours: dashboardData.avgResolutionHours ?? null,
    avgRating: dashboardData.avgRating ?? null,
    topProblemCategories: (dashboardData.topProblemTypes ?? []).map((p) => ({
      category: p.name,
      count: p.count,
    })),
    topAgentWorkload: (dashboardData.topAgentWorkload ?? []).map((a) => ({
      agentName: a.fullName ?? a.agentName ?? a.name ?? 'Unknown',
      ticketCount: a.resolvedCount ?? a.ticketCount ?? a.count ?? 0,
    })),
  }
  const response = await api.post('/ai/admin-insights', payload)
  return response.data
}
