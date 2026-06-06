import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { catalogoApi } from '@/api/catalogoApi'

const formSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório').max(255),
  operadoraId: z.string().min(1, 'Operadora é obrigatória'),
  tipoBeneficio: z.enum(['SAUDE', 'ODONTO', 'VIDA', 'OUTROS']),
  modalidade: z.enum(['COLETIVO_EMPRESARIAL', 'POR_ADESAO']),
  registroAnsProduto: z.string().optional().or(z.literal('')),
})

type FormData = z.infer<typeof formSchema>

export function ProdutoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: produto, isLoading } = useQuery({
    queryKey: ['produtos', id],
    queryFn: () => catalogoApi.obterProduto(id!),
    enabled: isEditing,
  })

  const { data: operadoras } = useQuery({
    queryKey: ['operadoras-list'],
    queryFn: () => catalogoApi.listarOperadoras({ pageSize: 1000 }),
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (produto) {
      setValue('nome', produto.nome)
      setValue('operadoraId', produto.operadoraId)
      setValue('tipoBeneficio', produto.tipoBeneficio)
      setValue('modalidade', produto.modalidade)
      setValue('registroAnsProduto', produto.registroAnsProduto || '')
    }
  }, [produto, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.criarProduto({
        nome: data.nome,
        operadoraId: data.operadoraId,
        tipoBeneficio: data.tipoBeneficio,
        modalidade: data.modalidade,
        registroAnsProduto: data.registroAnsProduto || undefined,
      }),
    onSuccess: () => navigate('/catalogo/produtos'),
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.atualizarProduto(id!, {
        nome: data.nome,
        tipoBeneficio: data.tipoBeneficio,
        modalidade: data.modalidade,
        registroAnsProduto: data.registroAnsProduto || undefined,
      }),
    onSuccess: () => navigate('/catalogo/produtos'),
  })

  const onSubmit = (data: FormData) => {
    if (isEditing) {
      updateMutation.mutate(data)
    } else {
      createMutation.mutate(data)
    }
  }

  if (isEditing && isLoading) return <div>Carregando...</div>

  return (
    <div className="p-6 max-w-2xl mx-auto">
      <h1 className="text-3xl font-bold mb-6">
        {isEditing ? 'Editar Produto' : 'Novo Produto'}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Nome</label>
          <input
            {...register('nome')}
            type="text"
            className="w-full px-4 py-2 border rounded"
          />
          {errors.nome && <span className="text-red-500 text-sm">{errors.nome.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Operadora</label>
          <select
            {...register('operadoraId')}
            disabled={isEditing}
            className="w-full px-4 py-2 border rounded disabled:bg-gray-100"
          >
            <option value="">Selecione...</option>
            {operadoras?.items.map((op) => (
              <option key={op.id} value={op.id}>
                {op.razaoSocial}
              </option>
            ))}
          </select>
          {errors.operadoraId && <span className="text-red-500 text-sm">{errors.operadoraId.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Tipo de Benefício</label>
          <select {...register('tipoBeneficio')} className="w-full px-4 py-2 border rounded">
            <option value="">Selecione...</option>
            <option value="SAUDE">Saúde</option>
            <option value="ODONTO">Odontologia</option>
            <option value="VIDA">Vida</option>
            <option value="OUTROS">Outros</option>
          </select>
          {errors.tipoBeneficio && <span className="text-red-500 text-sm">{errors.tipoBeneficio.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Modalidade</label>
          <select {...register('modalidade')} className="w-full px-4 py-2 border rounded">
            <option value="">Selecione...</option>
            <option value="COLETIVO_EMPRESARIAL">Coletivo Empresarial</option>
            <option value="POR_ADESAO">Por Adesão</option>
          </select>
          {errors.modalidade && <span className="text-red-500 text-sm">{errors.modalidade.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Registro ANS do Produto</label>
          <input
            {...register('registroAnsProduto')}
            type="text"
            className="w-full px-4 py-2 border rounded"
          />
        </div>

        <div className="flex gap-4">
          <button
            type="submit"
            className="bg-blue-500 text-white px-6 py-2 rounded hover:bg-blue-600"
          >
            {isEditing ? 'Atualizar' : 'Criar'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/catalogo/produtos')}
            className="bg-gray-500 text-white px-6 py-2 rounded hover:bg-gray-600"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
