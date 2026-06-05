import axiosInstance from './axiosInstance'

export interface LoginPayload {
  email: string
  password: string
  tenantSlug: string
}

export interface RegisterPayload {
  name: string
  email: string
  password: string
  tenantSlug: string
}

export interface AuthResponse {
  accessToken: string
  refreshToken?: string
  expiresIn: number
}

export interface ForgotPasswordPayload {
  email: string
  tenantSlug: string
}

export interface ResetPasswordPayload {
  token: string
  newPassword: string
}

export const authApi = {
  login: async (payload: LoginPayload): Promise<AuthResponse> => {
    const response = await axiosInstance.post('/auth/login', payload)
    return response.data
  },

  register: async (payload: RegisterPayload): Promise<AuthResponse> => {
    const response = await axiosInstance.post('/auth/register', payload)
    return response.data
  },

  logout: async (): Promise<void> => {
    await axiosInstance.post('/auth/logout')
  },

  refresh: async (refreshToken: string): Promise<AuthResponse> => {
    const response = await axiosInstance.post('/auth/refresh', { refreshToken })
    return response.data
  },

  getCurrentUser: async () => {
    const response = await axiosInstance.get('/auth/me')
    return response.data
  },

  forgotPassword: async (payload: ForgotPasswordPayload): Promise<{ message: string }> => {
    const response = await axiosInstance.post('/auth/forgot-password', payload)
    return response.data
  },

  resetPassword: async (payload: ResetPasswordPayload): Promise<{ message: string }> => {
    const response = await axiosInstance.post('/auth/reset-password', payload)
    return response.data
  },
}
