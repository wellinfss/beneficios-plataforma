import axios from 'axios'
import { useAuthStore } from '@/store/authStore'

const API_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'

const axiosInstance = axios.create({
  baseURL: API_URL,
  timeout: 30000,
})

const bareAxiosInstance = axios.create({
  baseURL: API_URL,
  timeout: 30000,
})

axiosInstance.interceptors.request.use(
  (config) => {
    const { accessToken, tenantSlug } = useAuthStore.getState()

    if (accessToken) {
      config.headers.Authorization = `Bearer ${accessToken}`
    }

    if (tenantSlug) {
      config.headers['X-Tenant-Id'] = tenantSlug
    }

    return config
  },
  (error) => Promise.reject(error),
)

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true

      try {
        const refreshToken = localStorage.getItem('refreshToken')
        if (refreshToken) {
          const response = await bareAxiosInstance.post('/auth/refresh', {
            refreshToken,
          })
          const { accessToken } = response.data
          useAuthStore.setState({ accessToken })
          originalRequest.headers.Authorization = `Bearer ${accessToken}`
          return axiosInstance(originalRequest)
        }
      } catch (refreshError) {
        await useAuthStore.getState().logout()
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  },
)

export default axiosInstance
