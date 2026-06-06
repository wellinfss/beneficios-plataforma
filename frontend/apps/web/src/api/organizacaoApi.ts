import axiosInstance from './axiosInstance'
import type {
  GrupoEconomicoDto,
  EstipulanteDto,
  SubestipulanteDto,
  EnderecoDto,
  TelefoneDto,
  EmailDto,
  HierarquiaEstipulanteDto,
  PagedResult,
} from '@shared/organizacaoTypes'

interface ApiResponse<T> {
  isSuccess: boolean
  value?: T
  error?: string
}

export const organizacaoApi = {
  // Grupos Econômicos
  listarGruposEconomicos: async (params?: {
    nome?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<GrupoEconomicoDto>> => {
    const response = await axiosInstance.get('/grupos-economicos', { params })
    const wrapped: ApiResponse<PagedResult<GrupoEconomicoDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterGrupoEconomico: async (id: string): Promise<GrupoEconomicoDto> => {
    const response = await axiosInstance.get(`/grupos-economicos/${id}`)
    const data: ApiResponse<GrupoEconomicoDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  criarGrupoEconomico: async (data: {
    nome: string
    cnpjRaiz: string
    responsavel: string
  }): Promise<GrupoEconomicoDto> => {
    const response = await axiosInstance.post('/grupos-economicos', data)
    const result: ApiResponse<GrupoEconomicoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarGrupoEconomico: async (
    id: string,
    data: {
      nome: string
      responsavel: string
    }
  ): Promise<GrupoEconomicoDto> => {
    const response = await axiosInstance.put(`/grupos-economicos/${id}`, data)
    const result: ApiResponse<GrupoEconomicoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirGrupoEconomico: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/grupos-economicos/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusGrupoEconomico: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<GrupoEconomicoDto> => {
    const response = await axiosInstance.patch(`/grupos-economicos/${id}/status`, { status })
    const result: ApiResponse<GrupoEconomicoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },

  // Estipulantes
  listarEstipulantes: async (params?: {
    nome?: string
    cnpj?: string
    grupoEconomicoId?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<EstipulanteDto>> => {
    const response = await axiosInstance.get('/estipulantes', { params })
    const wrapped: ApiResponse<PagedResult<EstipulanteDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterEstipulante: async (id: string): Promise<EstipulanteDto> => {
    const response = await axiosInstance.get(`/estipulantes/${id}`)
    const data: ApiResponse<EstipulanteDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  obterHierarquiaEstipulante: async (id: string): Promise<HierarquiaEstipulanteDto> => {
    const response = await axiosInstance.get(`/estipulantes/${id}/hierarquia`)
    const data: ApiResponse<HierarquiaEstipulanteDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch hierarchy')
    return data.value
  },

  criarEstipulante: async (data: {
    razaoSocial: string
    nomeFantasia?: string
    cnpj: string
    endereco: EnderecoDto
    telefone: TelefoneDto
    email: EmailDto
    grupoEconomicoId?: string
  }): Promise<EstipulanteDto> => {
    const response = await axiosInstance.post('/estipulantes', data)
    const result: ApiResponse<EstipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarEstipulante: async (
    id: string,
    data: {
      razaoSocial: string
      nomeFantasia?: string
      endereco: EnderecoDto
      telefone: TelefoneDto
      email: EmailDto
      grupoEconomicoId?: string
    }
  ): Promise<EstipulanteDto> => {
    const response = await axiosInstance.put(`/estipulantes/${id}`, data)
    const result: ApiResponse<EstipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirEstipulante: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/estipulantes/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusEstipulante: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<EstipulanteDto> => {
    const response = await axiosInstance.patch(`/estipulantes/${id}/status`, { status })
    const result: ApiResponse<EstipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },

  // Subestipulantes
  listarSubestipulantes: async (params?: {
    estipulanteId?: string
    nome?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<SubestipulanteDto>> => {
    const response = await axiosInstance.get('/subestipulantes', { params })
    const wrapped: ApiResponse<PagedResult<SubestipulanteDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterSubestipulante: async (id: string): Promise<SubestipulanteDto> => {
    const response = await axiosInstance.get(`/subestipulantes/${id}`)
    const data: ApiResponse<SubestipulanteDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  criarSubestipulante: async (data: {
    razaoSocial: string
    nomeFantasia?: string
    cnpj: string
    estipulanteId: string
    endereco?: EnderecoDto
    telefone?: TelefoneDto
    email?: EmailDto
  }): Promise<SubestipulanteDto> => {
    const response = await axiosInstance.post('/subestipulantes', data)
    const result: ApiResponse<SubestipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarSubestipulante: async (
    id: string,
    data: {
      razaoSocial: string
      nomeFantasia?: string
      endereco?: EnderecoDto
      telefone?: TelefoneDto
      email?: EmailDto
    }
  ): Promise<SubestipulanteDto> => {
    const response = await axiosInstance.put(`/subestipulantes/${id}`, data)
    const result: ApiResponse<SubestipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirSubestipulante: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/subestipulantes/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusSubestipulante: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<SubestipulanteDto> => {
    const response = await axiosInstance.patch(`/subestipulantes/${id}/status`, { status })
    const result: ApiResponse<SubestipulanteDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },
}
