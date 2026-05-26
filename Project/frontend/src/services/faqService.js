import api from './api'

export async function getFaqs() {
  const response = await api.get('/faq')
  return response.data
}

// PB-61 / US-104: Admin lista (uključuje neaktivne)
export async function getAllFaqs() {
  const response = await api.get('/faq/all')
  return response.data
}

export async function createFaq(payload) {
  const response = await api.post('/faq', payload)
  return response.data
}

export async function updateFaq(faqId, payload) {
  const response = await api.put(`/faq/${faqId}`, payload)
  return response.data
}

export async function deleteFaq(faqId) {
  const response = await api.delete(`/faq/${faqId}`)
  return response.data
}
