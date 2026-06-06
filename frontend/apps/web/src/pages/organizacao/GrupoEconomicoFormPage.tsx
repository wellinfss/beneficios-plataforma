import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { organizacaoApi } from '@/api/organizacaoApi'

const formSchema = z.object({
  nome: z.string().min(1, 'Nome é obrigatório').max(255),
  cnpjRaiz: z.string().regex(/^\d{8}$/, 'CNPJ Raiz deve ter exatamente 8 dígitos'),
  responsavel: z.string().min(1, 'Responsável é obrigatório').max(255),
})

type FormData = z.infer<typeof formSchema>

export function GrupoEconomicoFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: grupo, isLoading } = useQuery({
    queryKey: ['grupos-economicos', id],
    queryFn: () => organizacaoApi.obterGrupoEconomico(id!),
    enabled: isEditing,
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (grupo) {
      setValue('nome', grupo.nome)
      setValue('cnpjRaiz', grupo.cnpjRaiz)
      setValue('responsavel', grupo.responsavel)
    }
  }, [grupo, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) => organizacaoApi.criarGrupoEconomico(data),
    onSuccess: () => {
      navigate('/organizacao/grupos-economicos')
    },
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      organizacaoApi.atualizarGrupoEconomico(id!, {
        nome: data.nome,
        responsavel: data.responsavel,
      }),
    onSuccess: () => {
      navigate('/organizacao/grupos-economicos')
    },
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
        {isEditing ? 'Editar Grupo Econômico' : 'Novo Grupo Econômico'}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Nome</label>
          <input
            {...register('nome')}
            type="text"
            className="w-full px-4 py-2 border rounded"
            placeholder="Digite o nome do grupo"
          />
          {errors.nome && <span className="text-red-500 text-sm">{errors.nome.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">CNPJ Raiz</label>
          <input
            {...register('cnpjRaiz')}
            type="text"
            className="w-full px-4 py-2 border rounded"
            placeholder="Digite os 8 dígitos do CNPJ Raiz"
            disabled={isEditing}
          />
          {errors.cnpjRaiz && <span className="text-red-500 text-sm">{errors.cnpjRaiz.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Responsável</label>
          <input
            {...register('responsavel')}
            type="text"
            className="w-full px-4 py-2 border rounded"
            placeholder="Nome do responsável"
          />
          {errors.responsavel && (
            <span className="text-red-500 text-sm">{errors.responsavel.message}</span>
          )}
        </div>

        <div className="flex gap-4 mt-6">
          <button
            type="submit"
            disabled={createMutation.isPending || updateMutation.isPending}
            className="px-6 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:opacity-50"
          >
            {isEditing ? 'Atualizar' : 'Criar'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/organizacao/grupos-economicos')}
            className="px-6 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
