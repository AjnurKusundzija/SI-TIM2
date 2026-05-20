import api from './api'

// US-6: Dohvati aktivne pakete prijavljenog korisnika
export async function getMyPackages() {
  const response = await api.get('/packages')
  return response.data
}

// US-7: Detalji jednog paketa — backend provjerava vlasništvo
export async function getPackageById(packageId) {
  const response = await api.get(`/packages/${packageId}`)
  return response.data
}
