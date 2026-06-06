import { useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { organizacaoApi } from '@/api/organizacaoApi'

const formSchema = z.object({
  razaoSocial: z.string().min(1, 'Razão Social é obrigatória'),
  nomeFantasia: z.string().optional(),
  cnpj: z.string().regex(/^\d{14}$|^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/, 'CNPJ inválido'),
  estipulanteId: z.string().min(1, 'Estipulante é obrigatório'),
  logradouro: z.string().optional(),
  numero: z.string().optional(),
  complemento: z.string().optional(),
  bairro: z.string().optional(),
  cidade: z.string().optional(),
  uf: z.string().optional(),
  cep: z.string().optional(),
  telefone: z.string().optional(),
  email: z.string().email().optional().or(z.literal('')),
})

type FormData = z.infer<typeof formSchema>

export function SubestipulanteFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: subestipulante, isLoading } = useQuery({
    queryKey: ['subestipulantes', id],
    queryFn: () => organizacaoApi.obterSubestipulante(id!),
    enabled: isEditing,
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (subestipulante) {
      setValue('razaoSocial', subestipulante.razaoSocial)
      setValue('nomeFantasia', subestipulante.nomeFantasia)
      setValue('cnpj', subestipulante.cnpj)
      setValue('estipulanteId', subestipulante.estipulanteId)
      if (subestipulante.endereco) {
        setValue('logradouro', subestipulante.endereco.logradouro)
        setValue('numero', subestipulante.endereco.numero)
        setValue('complemento', subestipulante.endereco.complemento)
        setValue('bairro', subestipulante.endereco.bairro)
        setValue('cidade', subestipulante.endereco.cidade)
        setValue('uf', subestipulante.endereco.uf)
        setValue('cep', subestipulante.endereco.cep)
      }
      if (subestipulante.telefone) {
        setValue('telefone', subestipulante.telefone.numero)
      }
      if (subestipulante.email) {
        setValue('email', subestipulante.email.endereco)
      }
    }
  }, [subestipulante, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      organizacaoApi.criarSubestipulante({
        razaoSocial: data.razaoSocial,
        nomeFantasia: data.nomeFantasia,
        cnpj: data.cnpj,
        estipulanteId: data.estipulanteId,
        endereco: data.logradouro
          ? {
              logradouro: data.logradouro,
              numero: data.numero || '',
              complemento: data.complemento,
              bairro: data.bairro || '',
              cidade: data.cidade || '',
              uf: data.uf || '',
              cep: data.cep || '',
            }
          : undefined,
        telefone: data.telefone ? { numero: data.telefone } : undefined,
        email: data.email ? { endereco: data.email } : undefined,
      }),
    onSuccess: () => navigate('/organizacao/subestipulantes'),
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      organizacaoApi.atualizarSubestipulante(id!, {
        razaoSocial: data.razaoSocial,
        nomeFantasia: data.nomeFantasia,
        endereco: data.logradouro
          ? {
              logradouro: data.logradouro,
              numero: data.numero || '',
              complemento: data.complemento,
              bairro: data.bairro || '',
              cidade: data.cidade || '',
              uf: data.uf || '',
              cep: data.cep || '',
            }
          : undefined,
        telefone: data.telefone ? { numero: data.telefone } : undefined,
        email: data.email ? { endereco: data.email } : undefined,
      }),
    onSuccess: () => navigate('/organizacao/subestipulantes'),
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
    <div className="p-6 max-w-4xl mx-auto">
      <h1 className="text-3xl font-bold mb-6">
        {isEditing ? 'Editar Subestipulante' : 'Novo Subestipulante'}
      </h1>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <fieldset className="border p-4 rounded">
          <legend className="text-lg font-semibold px-2">Informações Básicas</legend>
          <div className="grid grid-cols-2 gap-4 mt-4">
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
              <label className="block text-sm font-medium mb-1">Nome Fantasia</label>
              <input
                {...register('nomeFantasia')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">CNPJ</label>
              <input
                {...register('cnpj')}
                type="text"
                className="w-full px-4 py-2 border rounded"
                disabled={isEditing}
              />
              {errors.cnpj && <span className="text-red-500 text-sm">{errors.cnpj.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Estipulante</label>
              <input
                {...register('estipulanteId')}
                type="text"
                className="w-full px-4 py-2 border rounded"
                placeholder="ID do Estipulante"
                disabled={isEditing}
              />
              {errors.estipulanteId && (
                <span className="text-red-500 text-sm">{errors.estipulanteId.message}</span>
              )}
            </div>
          </div>
        </fieldset>

        <fieldset className="border p-4 rounded">
          <legend className="text-lg font-semibold px-2">Endereço (Opcional)</legend>
          <div className="grid grid-cols-2 gap-4 mt-4">
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-1">Logradouro</label>
              <input
                {...register('logradouro')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Número</label>
              <input
                {...register('numero')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Complemento</label>
              <input
                {...register('complemento')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Bairro</label>
              <input
                {...register('bairro')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Cidade</label>
              <input
                {...register('cidade')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">UF</label>
              <input
                {...register('uf')}
                type="text"
                maxLength={2}
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">CEP</label>
              <input
                {...register('cep')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>
          </div>
        </fieldset>

        <fieldset className="border p-4 rounded">
          <legend className="text-lg font-semibold px-2">Contato (Opcional)</legend>
          <div className="grid grid-cols-2 gap-4 mt-4">
            <div>
              <label className="block text-sm font-medium mb-1">Telefone</label>
              <input
                {...register('telefone')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Email</label>
              <input
                {...register('email')}
                type="email"
                className="w-full px-4 py-2 border rounded"
              />
            </div>
          </div>
        </fieldset>

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
            onClick={() => navigate('/organizacao/subestipulantes')}
            className="px-6 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
