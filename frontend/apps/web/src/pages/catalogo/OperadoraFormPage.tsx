import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { catalogoApi } from '@/api/catalogoApi'

const formSchema = z.object({
  razaoSocial: z.string().min(1, 'Razão Social é obrigatória').max(255),
  cnpj: z.string().regex(/^\d{14}$|^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/, 'CNPJ inválido'),
  tipo: z.enum(['SAUDE', 'ODONTO', 'VIDA', 'OUTROS']),
  registroAns: z.string().regex(/^\d{6}$/, 'Registro ANS deve ter 6 dígitos').optional().or(z.literal('')),
  endpointIntegracao: z.string().url().optional().or(z.literal('')),
  formatoIntegracao: z.string().optional(),
  credenciais: z.string().optional(),
})

type FormData = z.infer<typeof formSchema>

export function OperadoraFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: operadora, isLoading } = useQuery({
    queryKey: ['operadoras', id],
    queryFn: () => catalogoApi.obterOperadora(id!),
    enabled: isEditing,
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (operadora) {
      setValue('razaoSocial', operadora.razaoSocial)
      setValue('cnpj', operadora.cnpj)
      setValue('tipo', operadora.tipo)
      setValue('registroAns', operadora.registroAns || '')
      setValue('endpointIntegracao', operadora.endpointIntegracao || '')
      setValue('formatoIntegracao', operadora.formatoIntegracao || '')
    }
  }, [operadora, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.criarOperadora({
        razaoSocial: data.razaoSocial,
        cnpj: data.cnpj,
        tipo: data.tipo,
        registroAns: data.registroAns || undefined,
      }),
    onSuccess: () => navigate('/catalogo/operadoras'),
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.atualizarOperadora(id!, {
        razaoSocial: data.razaoSocial,
        tipo: data.tipo,
        registroAns: data.registroAns || undefined,
      }),
    onSuccess: (_, data) => {
      if (data.endpointIntegracao || data.formatoIntegracao || data.credenciais) {
        integrationMutation.mutate(data)
      } else {
        navigate('/catalogo/operadoras')
      }
    },
  })

  const integrationMutation = useMutation({
    mutationFn: (data: FormData) =>
      catalogoApi.atualizarIntegracaoOperadora(id!, {
        endpointIntegracao: data.endpointIntegracao || undefined,
        formatoIntegracao: data.formatoIntegracao || undefined,
        credenciais: data.credenciais || undefined,
      }),
    onSuccess: () => navigate('/catalogo/operadoras'),
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
        {isEditing ? 'Editar Operadora' : 'Nova Operadora'}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <label className="block text-sm font-medium mb-1">Razão Social</label>
          <input
            {...register('razaoSocial')}
            type="text"
            className="w-full px-4 py-2 border rounded"
          />
          {errors.razaoSocial && <span className="text-red-500 text-sm">{errors.razaoSocial.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">CNPJ</label>
          <input
            {...register('cnpj')}
            type="text"
            disabled={isEditing}
            className="w-full px-4 py-2 border rounded disabled:bg-gray-100"
          />
          {errors.cnpj && <span className="text-red-500 text-sm">{errors.cnpj.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Tipo</label>
          <select {...register('tipo')} className="w-full px-4 py-2 border rounded">
            <option value="">Selecione...</option>
            <option value="SAUDE">Saúde</option>
            <option value="ODONTO">Odontologia</option>
            <option value="VIDA">Vida</option>
            <option value="OUTROS">Outros</option>
          </select>
          {errors.tipo && <span className="text-red-500 text-sm">{errors.tipo.message}</span>}
        </div>

        <div>
          <label className="block text-sm font-medium mb-1">Registro ANS</label>
          <input
            {...register('registroAns')}
            type="text"
            placeholder="6 dígitos"
            className="w-full px-4 py-2 border rounded"
          />
          {errors.registroAns && <span className="text-red-500 text-sm">{errors.registroAns.message}</span>}
        </div>

        {isEditing && (
          <>
            <hr className="my-6" />
            <h2 className="text-xl font-bold mb-4">Integração</h2>

            <div>
              <label className="block text-sm font-medium mb-1">Endpoint de Integração</label>
              <input
                {...register('endpointIntegracao')}
                type="text"
                placeholder="https://..."
                className="w-full px-4 py-2 border rounded"
              />
              {errors.endpointIntegracao && <span className="text-red-500 text-sm">{errors.endpointIntegracao.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Formato de Integração</label>
              <input
                {...register('formatoIntegracao')}
                type="text"
                placeholder="ex: JSON, XML"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Credenciais</label>
              <textarea
                {...register('credenciais')}
                className="w-full px-4 py-2 border rounded"
                rows={4}
                placeholder="Insira as credenciais em texto plano (serão encriptadas)"
              />
            </div>
          </>
        )}

        <div className="flex gap-4">
          <button
            type="submit"
            className="bg-blue-500 text-white px-6 py-2 rounded hover:bg-blue-600"
          >
            {isEditing ? 'Atualizar' : 'Criar'}
          </button>
          <button
            type="button"
            onClick={() => navigate('/catalogo/operadoras')}
            className="bg-gray-500 text-white px-6 py-2 rounded hover:bg-gray-600"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
