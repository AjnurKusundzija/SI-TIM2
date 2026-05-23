import api from './api'

// PB-52 / US-76: Lista svih paketa za admina (uključuje neaktivne + broj aktivnih pretplata).
export async function getCatalog() {
  const response = await api.get('/packages/catalog')
  return response.data
}

// PB-52 / US-77: Lista aktivnih paketa — koristi se u modal-u za dodjelu.
export async function getActiveCatalog() {
  const response = await api.get('/packages/catalog/active')
  return response.data
}

export async function createCatalogPackage(payload) {
  const response = await api.post('/packages', payload)
  return response.data
}

export async function updateCatalogPackage(id, payload) {
  const response = await api.put(`/packages/${id}`, payload)
  return response.data
}

export async function deleteCatalogPackage(id) {
  const response = await api.delete(`/packages/${id}`)
  return response.data
}

export async function updateCatalogPackageStatus(id, status) {
  const response = await api.patch(`/packages/${id}/status`, { status })
  return response.data
}

// PB-52 / US-77: Pretplate jednog klijenta.
export async function getClientSubscriptions(clientId) {
  const response = await api.get(`/clients/${clientId}/subscriptions`)
  return response.data
}

export async function assignSubscription(clientId, catalogPackageId, startDate) {
  const response = await api.post(`/clients/${clientId}/subscriptions`, {
    catalogPackageId,
    startDate,
  })
  return response.data
}

export async function deactivateSubscription(clientId, subscriptionId) {
  const response = await api.patch(
    `/clients/${clientId}/subscriptions/${subscriptionId}/deactivate`
  )
  return response.data
}
