import axios from 'axios'

const api = axios.create({
  baseURL: '/api',
})

api.interceptors.request.use((config) => {
  const token = sessionStorage.getItem('accessToken')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

let isRefreshing = false
let failedQueue = []

function processQueue(error, token = null) {
  failedQueue.forEach((p) => (error ? p.reject(error) : p.resolve(token)))
  failedQueue = []
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config

    if (error.response?.status !== 401 || original._retry) {
      throw error
    }

    // login/refresh endpoints — propagate error directly to the caller
    if (original.url?.includes('/auth/login') || original.url?.includes('/auth/refresh')) {
      if (original.url?.includes('/auth/refresh')) {
        sessionStorage.clear()
        globalThis.location.href = '/login'
      }
      throw error
    }

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject })
      }).then((token) => {
        original.headers.Authorization = `Bearer ${token}`
        return api(original)
      })
    }

    original._retry = true
    isRefreshing = true

    const refreshToken = sessionStorage.getItem('refreshToken')
    if (!refreshToken) {
      sessionStorage.clear()
      globalThis.location.href = '/login'
      throw error
    }

    try {
      const { data } = await axios.post('/api/auth/refresh', { refreshToken })
      sessionStorage.setItem('accessToken', data.accessToken)
      sessionStorage.setItem('refreshToken', data.refreshToken)
      api.defaults.headers.common.Authorization = `Bearer ${data.accessToken}`
      processQueue(null, data.accessToken)
      original.headers.Authorization = `Bearer ${data.accessToken}`
      return api(original)
    } catch (refreshError) {
      processQueue(refreshError, null)
      sessionStorage.clear()
      globalThis.location.href = '/login'
      return Promise.reject(refreshError)
    } finally {
      isRefreshing = false
    }
  }
)

export default api
