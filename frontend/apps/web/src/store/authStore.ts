import { create } from 'zustand'
import axiosInstance from '@/api/axiosInstance'

interface User {
  id: string
  email: string
  name: string
  tenantId: string
}

interface AuthStore {
  user: User | null
  accessToken: string | null
  tenantSlug: string | null
  isAuthenticated: boolean
  login: (email: string, password: string, tenantSlug: string) => Promise<void>
  logout: () => void
  setUser: (user: User) => void
  setTenantSlug: (slug: string) => void
}

export const useAuthStore = create<AuthStore>((set) => ({
  user: null,
  accessToken: null,
  tenantSlug: null,
  isAuthenticated: false,

  login: async (email: string, password: string, tenantSlug: string) => {
    try {
      const response = await axiosInstance.post('/auth/login', {
        email,
        password,
        tenantSlug,
      })

      const { accessToken, refreshToken } = response.data

      set({
        accessToken,
        tenantSlug,
        isAuthenticated: true,
      })

      if (refreshToken) {
        localStorage.setItem('refreshToken', refreshToken)
      }

      const userResponse = await axiosInstance.get('/auth/me')
      set({
        user: userResponse.data,
      })
    } catch (error) {
      set({
        user: null,
        accessToken: null,
        isAuthenticated: false,
      })
      throw error
    }
  },

  logout: () => {
    set({
      user: null,
      accessToken: null,
      tenantSlug: null,
      isAuthenticated: false,
    })
    localStorage.removeItem('refreshToken')
  },

  setUser: (user: User) => {
    set({ user })
  },

  setTenantSlug: (slug: string) => {
    set({ tenantSlug: slug })
  },
}))
