import axiosInstance from './axiosInstance'
import type {
  OperadoraDto,
  ProdutoDto,
  PlanoDto,
  IntegracaoOperadoraDto,
  PagedResult,
} from '@shared/catalogoTypes'

interface ApiResponse<T> {
  isSuccess: boolean
  value?: T
  error?: string
}

export const catalogoApi = {
  // Operadoras
  listarOperadoras: async (params?: {
    razaoSocial?: string
    tipo?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<OperadoraDto>> => {
    const response = await axiosInstance.get('/operadoras', { params })
    const wrapped: ApiResponse<PagedResult<OperadoraDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterOperadora: async (id: string): Promise<OperadoraDto> => {
    const response = await axiosInstance.get(`/operadoras/${id}`)
    const data: ApiResponse<OperadoraDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  listarProdutosPorOperadora: async (operadoraId: string): Promise<ProdutoDto[]> => {
    const response = await axiosInstance.get(`/operadoras/${operadoraId}/produtos`)
    const data: ApiResponse<ProdutoDto[]> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  criarOperadora: async (data: {
    razaoSocial: string
    cnpj: string
    tipo: string
    registroAns?: string
  }): Promise<OperadoraDto> => {
    const response = await axiosInstance.post('/operadoras', data)
    const result: ApiResponse<OperadoraDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarOperadora: async (
    id: string,
    data: {
      razaoSocial: string
      tipo: string
      registroAns?: string
    }
  ): Promise<OperadoraDto> => {
    const response = await axiosInstance.put(`/operadoras/${id}`, data)
    const result: ApiResponse<OperadoraDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirOperadora: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/operadoras/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusOperadora: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<OperadoraDto> => {
    const response = await axiosInstance.patch(`/operadoras/${id}/status`, { status })
    const result: ApiResponse<OperadoraDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },

  atualizarIntegracaoOperadora: async (
    id: string,
    data: IntegracaoOperadoraDto
  ): Promise<OperadoraDto> => {
    const response = await axiosInstance.put(`/operadoras/${id}/integracao`, {
      endpointIntegracao: data.endpointIntegracao,
      formatoIntegracao: data.formatoIntegracao,
      credenciaisPlanoTexto: data.credenciais,
    })
    const result: ApiResponse<OperadoraDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  // Produtos
  listarProdutos: async (params?: {
    nome?: string
    operadoraId?: string
    tipoBeneficio?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<ProdutoDto>> => {
    const response = await axiosInstance.get('/produtos', { params })
    const wrapped: ApiResponse<PagedResult<ProdutoDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterProduto: async (id: string): Promise<ProdutoDto> => {
    const response = await axiosInstance.get(`/produtos/${id}`)
    const data: ApiResponse<ProdutoDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  listarPlanosPorProduto: async (produtoId: string): Promise<PlanoDto[]> => {
    const response = await axiosInstance.get(`/produtos/${produtoId}/planos`)
    const data: ApiResponse<PlanoDto[]> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  criarProduto: async (data: {
    nome: string
    operadoraId: string
    tipoBeneficio: string
    modalidade: string
    registroAnsProduto?: string
  }): Promise<ProdutoDto> => {
    const response = await axiosInstance.post('/produtos', data)
    const result: ApiResponse<ProdutoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarProduto: async (
    id: string,
    data: {
      nome: string
      tipoBeneficio: string
      modalidade: string
      registroAnsProduto?: string
    }
  ): Promise<ProdutoDto> => {
    const response = await axiosInstance.put(`/produtos/${id}`, data)
    const result: ApiResponse<ProdutoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirProduto: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/produtos/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusProduto: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<ProdutoDto> => {
    const response = await axiosInstance.patch(`/produtos/${id}/status`, { status })
    const result: ApiResponse<ProdutoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },

  // Planos
  listarPlanos: async (params?: {
    nome?: string
    operadoraId?: string
    tipoBeneficio?: string
    status?: string
    page?: number
    pageSize?: number
  }): Promise<PagedResult<PlanoDto>> => {
    const response = await axiosInstance.get('/planos', { params })
    const wrapped: ApiResponse<PagedResult<PlanoDto>> = response.data
    if (!wrapped.isSuccess || !wrapped.value) throw new Error(wrapped.error || 'Failed to fetch')
    return wrapped.value
  },

  obterPlano: async (id: string): Promise<PlanoDto> => {
    const response = await axiosInstance.get(`/planos/${id}`)
    const data: ApiResponse<PlanoDto> = response.data
    if (!data.isSuccess || !data.value) throw new Error(data.error || 'Failed to fetch')
    return data.value
  },

  criarPlano: async (data: {
    nome: string
    produtoId: string
    cobertura?: string
    abrangenciaGeografica?: string
    tipoAcomodacao?: string
    valorReferencia?: number
  }): Promise<PlanoDto> => {
    const response = await axiosInstance.post('/planos', data)
    const result: ApiResponse<PlanoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to create')
    return result.value
  },

  atualizarPlano: async (
    id: string,
    data: {
      nome: string
      cobertura?: string
      abrangenciaGeografica?: string
      tipoAcomodacao?: string
      valorReferencia?: number
    }
  ): Promise<PlanoDto> => {
    const response = await axiosInstance.put(`/planos/${id}`, data)
    const result: ApiResponse<PlanoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update')
    return result.value
  },

  excluirPlano: async (id: string): Promise<void> => {
    const response = await axiosInstance.delete(`/planos/${id}`)
    const result: ApiResponse<boolean> = response.data
    if (!result.isSuccess) throw new Error(result.error || 'Failed to delete')
  },

  alterarStatusPlano: async (
    id: string,
    status: 'ATIVO' | 'INATIVO'
  ): Promise<PlanoDto> => {
    const response = await axiosInstance.patch(`/planos/${id}/status`, { status })
    const result: ApiResponse<PlanoDto> = response.data
    if (!result.isSuccess || !result.value) throw new Error(result.error || 'Failed to update status')
    return result.value
  },
}
