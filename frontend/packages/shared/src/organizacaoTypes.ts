export interface GrupoEconomicoDto {
  id: string
  nome: string
  cnpjRaiz: string
  responsavel: string
  status: 'ATIVO' | 'INATIVO'
  createdAt: string
  updatedAt: string
}

export interface EstipulanteDto {
  id: string
  razaoSocial: string
  nomeFantasia?: string
  cnpj: string
  endereco: EnderecoDto
  telefone: TelefoneDto
  email: EmailDto
  grupoEconomicoId?: string
  grupoEconomicoNome?: string
  status: 'ATIVO' | 'INATIVO'
  createdAt: string
  updatedAt: string
}

export interface SubestipulanteDto {
  id: string
  razaoSocial: string
  nomeFantasia?: string
  cnpj: string
  endereco?: EnderecoDto
  telefone?: TelefoneDto
  email?: EmailDto
  estipulanteId: string
  estipulanteRazaoSocial?: string
  status: 'ATIVO' | 'INATIVO'
  createdAt: string
  updatedAt: string
}

export interface EnderecoDto {
  logradouro: string
  numero: string
  complemento?: string
  bairro: string
  cidade: string
  uf: string
  cep: string
}

export interface TelefoneDto {
  numero: string
}

export interface EmailDto {
  endereco: string
}

export interface HierarquiaEstipulanteDto {
  estipulante: EstipulanteDto
  subestipulantes: SubestipulanteDto[]
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

export const GRUPOS_ECONOMICOS_READ = 'grupos-economicos:read'
export const GRUPOS_ECONOMICOS_WRITE = 'grupos-economicos:write'
export const ESTIPULANTES_READ = 'estipulantes:read'
export const ESTIPULANTES_WRITE = 'estipulantes:write'
export const SUBESTIPULANTES_READ = 'subestipulantes:read'
export const SUBESTIPULANTES_WRITE = 'subestipulantes:write'
