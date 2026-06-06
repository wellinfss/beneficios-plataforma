export interface OperadoraDto {
  id: string
  razaoSocial: string
  cnpj: string
  registroAns?: string
  tipo: 'SAUDE' | 'ODONTO' | 'VIDA' | 'OUTROS'
  status: 'ATIVO' | 'INATIVO'
  endpointIntegracao?: string
  formatoIntegracao?: string
  createdAt: string
  updatedAt: string
}

export interface ProdutoDto {
  id: string
  nome: string
  operadoraId: string
  operadoraNome?: string
  tipoBeneficio: 'SAUDE' | 'ODONTO' | 'VIDA' | 'OUTROS'
  modalidade: 'COLETIVO_EMPRESARIAL' | 'POR_ADESAO'
  registroAnsProduto?: string
  status: 'ATIVO' | 'INATIVO'
  createdAt: string
  updatedAt: string
}

export interface PlanoDto {
  id: string
  nome: string
  produtoId: string
  produtoNome?: string
  operadoraId?: string
  operadoraNome?: string
  cobertura?: string
  abrangenciaGeografica?: string
  tipoAcomodacao?: 'ENFERMARIA' | 'APARTAMENTO' | 'OUTROS'
  valorReferencia?: number
  status: 'ATIVO' | 'INATIVO'
  createdAt: string
  updatedAt: string
}

export interface IntegracaoOperadoraDto {
  endpointIntegracao?: string
  formatoIntegracao?: string
  credenciais?: string
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  pageNumber: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export const OPERADORAS_READ = 'operadoras:read'
export const OPERADORAS_WRITE = 'operadoras:write'
export const PRODUTOS_READ = 'produtos:read'
export const PRODUTOS_WRITE = 'produtos:write'
export const PLANOS_READ = 'planos:read'
export const PLANOS_WRITE = 'planos:write'
