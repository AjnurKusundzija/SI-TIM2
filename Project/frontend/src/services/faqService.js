import api from './api'

export async function getFaqs() {
  const response = await api.get('/faq')
  return response.data
}
