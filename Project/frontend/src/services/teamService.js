import api from './api'

// US-24: Get all teams overview (admin only)
export async function getTeamsOverview() {
  const response = await api.get('/teams')
  return response.data
}

// US-24: Reassign an agent to a new team (admin only)
export async function reassignAgent(agentId, newTeamId) {
  const response = await api.post('/teams/reassign', { agentId, newTeamId })
  return response.data
}
