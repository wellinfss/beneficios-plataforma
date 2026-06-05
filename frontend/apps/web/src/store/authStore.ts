import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import axiosInstance from '@/api/axiosInstance'
import { authApi } from '@/api/authApi'
import type { UserDto } from '@beneficios-plataforma/shared'

interface AuthStore {
  user: UserDto | null
  accessToken: string | null
  tenantSlug: string | null
  isAuthenticated: boolean
  login: (email: string, password: string, tenantSlug: string) => Promise<void>
  logout: () => Promise<void>
  setUser: (user: UserDto) => void
  setTenantSlug: (slug: string) => void
  setAccessToken: (token: string) => void
}

export const useAuthStore = create<AuthStore>(
  persist(
    (set) => ({
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

      logout: async () => {
        try {
          await authApi.logout()
        } catch (error) {
          console.error('Error calling logout API:', error)
        } finally {
          set({
            user: null,
            accessToken: null,
            tenantSlug: null,
            isAuthenticated: false,
          })
          localStorage.removeItem('refreshToken')
          localStorage.removeItem('auth-storage')
        }
      },

      setUser: (user: UserDto) => {
        set({ user })
      },

      setTenantSlug: (slug: string) => {
        set({ tenantSlug: slug })
      },

      setAccessToken: (token: string) => {
        set({ accessToken: token })
      },
    }),
    {
      name: 'auth-storage',
      partialize: (state) => ({
        user: state.user,
        accessToken: state.accessToken,
        tenantSlug: state.tenantSlug,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
)
