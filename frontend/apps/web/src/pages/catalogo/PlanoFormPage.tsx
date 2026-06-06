import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { catalogoApi } from '@/api/catalogoApi'

const formSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório').max(255),
  produtoId: z.string().min(1, 'Produto é obrigatório'),
  cobertura: z.string().optional(),
  abrangenciaGeografica: z.string().optional(),
  tipoAcomodacao: z.enum(['ENFERMARIA', 'APARTAMENTO', 'OUTROS']).optional().or(z.literal('')),
  valorReferencia: z.coerce.number().positive().optional().or(z.literal('')),
})

type FormData = z.infer<typeof formSchema>

export function PlanoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: plano, isLoading } = useQuery({
    queryKey: ['planos', id],
    queryFn: () => catalogoApi.obterPlano(id!),
    enabled: isEditing,
  })

  const { data: produtos } = useQuery({
    queryKey: ['produtos-list'],
    queryFn: () => catalogoApi.listarProdutos({ pageSize: 1000 }),
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (plano) {
      setValue('nome', plano.nome)
      setValue('produtoId', plano.produtoId)
      setValue('cobertura', plano.cobertura || '')
      setValue('abrangenciaGeografica', plano.abrangenciaGeografica || '')
      setValue('tipoAcomodacao', plano.tipoAcomodacao || '')
      setValue('valorReferencia', plano.valorReferencia || '')
    }
  }, [plano, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.criarPlano({
        nome: data.nome,
        produtoId: data.produtoId,
        cobertura: data.cobertura || undefined,
        abrangenciaGeografica: data.abrangenciaGeografica || undefined,
        tipoAcomodacao: data.tipoAcomodacao || undefined,
        valorReferencia: data.valorReferencia || undefined,
      }),
    onSuccess: () => navigate('/catalogo/planos'),
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.atualizarPlano(id!, {
        nome: data.nome,
        cobertura: data.cobertura || undefined,
        abrangenciaGeografica: data.abrangenciaGeografica || undefined,
        tipoAcomodacao: data.tipoAcomodacao || undefined,
        valorReferencia: data.valorReferencia || undefined,
      }),
    onSuccess: () => navigate('/catalogo/planos'),
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
        {isEditing ? 'Editar Plano' : 'Novo Plano'}
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
          <label className="block text-sm font-medium mb-1">Produto</label>
          <select
            {...register('produtoId')}
            disabled={isEditing}
            className="w-full px-4 py-2 border rounded disabled:bg-gray-100"
          >
            <option value="">Selecione...</option>
            {produtos?.items.map((prod) => (
              <option key={prod.id} value={prod.id}>
                {prod.nome}
              </option>
            ))}
          </select>
          {errors.produtoId && <span className="text-red-500 text-sm">{errors.produtoId.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Cobertura</label>
          <input
            {...register('cobertura')}
            type="text"
            className="w-full px-4 py-2 border rounded"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Abrangência Geográfica</label>
          <input
            {...register('abrangenciaGeografica')}
            type="text"
            className="w-full px-4 py-2 border rounded"
          />
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Tipo de Acomodação</label>
          <select {...register('tipoAcomodacao')} className="w-full px-4 py-2 border rounded">
            <option value="">Selecione...</option>
            <option value="ENFERMARIA">Enfermaria</option>
            <option value="APARTAMENTO">Apartamento</option>
            <option value="OUTROS">Outros</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Valor de Referência</label>
          <input
            {...register('valorReferencia')}
            type="number"
            step="0.01"
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
            onClick={() => navigate('/catalogo/planos')}
            className="bg-gray-500 text-white px-6 py-2 rounded hover:bg-gray-600"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
