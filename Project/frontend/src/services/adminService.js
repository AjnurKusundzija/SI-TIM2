import api from './api'

export async function getAdminDashboard({ period = 'month', from, to } = {}) {
  const params = { period }
  if (period === 'custom' && from && to) {
    params.from = from
    params.to = to
  }
  const response = await api.get('/admin/dashboard', { params })
  return response.data
}

export async function generateReport({ reportType, period = 'month', from, to }) {
  const response = await api.post('/reports/generate', {
    reportType,
    period,
    from: period === 'custom' ? from : null,
    to: period === 'custom' ? to : null,
  })
  return response.data
}
