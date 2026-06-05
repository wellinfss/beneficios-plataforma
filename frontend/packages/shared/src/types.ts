export interface AuthResponseDto {
  accessToken: string
  refreshToken?: string
  expiresIn: number
}

export interface UserDto {
  id: string
  email: string
  name: string
  tenantId: string
  roles?: string[]
  permissions?: string[]
}

export interface TenantDto {
  id: string
  name: string
  slug: string
  status: string
}

export interface PagedResultDto<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export const PERMISSIONS = {
  BENEFICIARIOS_READ: 'beneficiarios:read',
  BENEFICIARIOS_WRITE: 'beneficiarios:write',
  CONTRATOS_READ: 'contratos:read',
  CONTRATOS_WRITE: 'contratos:write',
  OPERADORAS_READ: 'operadoras:read',
  OPERADORAS_WRITE: 'operadoras:write',
  AUDITORIA_READ: 'auditoria:read',
} as const
