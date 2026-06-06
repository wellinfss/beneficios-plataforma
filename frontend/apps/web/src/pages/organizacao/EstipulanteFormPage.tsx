import { useEffect, useState } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { useForm, Controller } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { useMutation, useQuery } from '@tanstack/react-query'
import { organizacaoApi } from '@/api/organizacaoApi'
import type { EnderecoDto, TelefoneDto, EmailDto } from '@shared/organizacaoTypes'

const formSchema = z.object({
  razaoSocial: z.string().min(1, 'Razão Social é obrigatória'),
  nomeFantasia: z.string().optional(),
  cnpj: z.string().regex(/^\d{14}$|^\d{2}\.\d{3}\.\d{3}\/\d{4}-\d{2}$/, 'CNPJ inválido'),
  logradouro: z.string().min(1, 'Logradouro é obrigatório'),
  numero: z.string().min(1, 'Número é obrigatório'),
  complemento: z.string().optional(),
  bairro: z.string().min(1, 'Bairro é obrigatório'),
  cidade: z.string().min(1, 'Cidade é obrigatória'),
  uf: z.string().length(2, 'UF deve ter 2 caracteres'),
  cep: z.string().regex(/^\d+$/, 'CEP deve conter apenas dígitos'),
  telefone: z.string().min(1, 'Telefone é obrigatório'),
  email: z.string().email('Email inválido'),
  grupoEconomicoId: z.string().optional(),
})

type FormData = z.infer<typeof formSchema>

export function EstipulanteFormPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const isEditing = Boolean(id && id !== 'novo')

  const { data: estipulante, isLoading } = useQuery({
    queryKey: ['estipulantes', id],
    queryFn: () => organizacaoApi.obterEstipulante(id!),
    enabled: isEditing,
  })

  const { register, handleSubmit, setValue, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
  })

  useEffect(() => {
    if (estipulante) {
      setValue('razaoSocial', estipulante.razaoSocial)
      setValue('nomeFantasia', estipulante.nomeFantasia)
      setValue('cnpj', estipulante.cnpj)
      setValue('logradouro', estipulante.endereco.logradouro)
      setValue('numero', estipulante.endereco.numero)
      setValue('complemento', estipulante.endereco.complemento)
      setValue('bairro', estipulante.endereco.bairro)
      setValue('cidade', estipulante.endereco.cidade)
      setValue('uf', estipulante.endereco.uf)
      setValue('cep', estipulante.endereco.cep)
      setValue('telefone', estipulante.telefone.numero)
      setValue('email', estipulante.email.endereco)
      setValue('grupoEconomicoId', estipulante.grupoEconomicoId)
    }
  }, [estipulante, setValue])

  const createMutation = useMutation({
    mutationFn: (data: FormData) =>
      organizacaoApi.criarEstipulante({
        razaoSocial: data.razaoSocial,
        nomeFantasia: data.nomeFantasia,
        cnpj: data.cnpj,
        endereco: {
          logradouro: data.logradouro,
          numero: data.numero,
          complemento: data.complemento,
          bairro: data.bairro,
          cidade: data.cidade,
          uf: data.uf,
          cep: data.cep,
        },
        telefone: { numero: data.telefone },
        email: { endereco: data.email },
        grupoEconomicoId: data.grupoEconomicoId || undefined,
      }),
    onSuccess: () => navigate('/organizacao/estipulantes'),
  })

  const updateMutation = useMutation({
    mutationFn: (data: FormData) =>
      organizacaoApi.atualizarEstipulante(id!, {
        razaoSocial: data.razaoSocial,
        nomeFantasia: data.nomeFantasia,
        endereco: {
          logradouro: data.logradouro,
          numero: data.numero,
          complemento: data.complemento,
          bairro: data.bairro,
          cidade: data.cidade,
          uf: data.uf,
          cep: data.cep,
        },
        telefone: { numero: data.telefone },
        email: { endereco: data.email },
        grupoEconomicoId: data.grupoEconomicoId || undefined,
      }),
    onSuccess: () => navigate('/organizacao/estipulantes'),
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
        {isEditing ? 'Editar Estipulante' : 'Novo Estipulante'}
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
          </div>
        </fieldset>

        <fieldset className="border p-4 rounded">
          <legend className="text-lg font-semibold px-2">Endereço</legend>
          <div className="grid grid-cols-2 gap-4 mt-4">
            <div className="col-span-2">
              <label className="block text-sm font-medium mb-1">Logradouro</label>
              <input
                {...register('logradouro')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.logradouro && (
                <span className="text-red-500 text-sm">{errors.logradouro.message}</span>
              )}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Número</label>
              <input
                {...register('numero')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.numero && <span className="text-red-500 text-sm">{errors.numero.message}</span>}
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
              {errors.bairro && <span className="text-red-500 text-sm">{errors.bairro.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Cidade</label>
              <input
                {...register('cidade')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.cidade && <span className="text-red-500 text-sm">{errors.cidade.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">UF</label>
              <input
                {...register('uf')}
                type="text"
                maxLength={2}
                className="w-full px-4 py-2 border rounded"
              />
              {errors.uf && <span className="text-red-500 text-sm">{errors.uf.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">CEP</label>
              <input
                {...register('cep')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.cep && <span className="text-red-500 text-sm">{errors.cep.message}</span>}
            </div>
          </div>
        </fieldset>

        <fieldset className="border p-4 rounded">
          <legend className="text-lg font-semibold px-2">Contato</legend>
          <div className="grid grid-cols-2 gap-4 mt-4">
            <div>
              <label className="block text-sm font-medium mb-1">Telefone</label>
              <input
                {...register('telefone')}
                type="text"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.telefone && <span className="text-red-500 text-sm">{errors.telefone.message}</span>}
            </div>

            <div>
              <label className="block text-sm font-medium mb-1">Email</label>
              <input
                {...register('email')}
                type="email"
                className="w-full px-4 py-2 border rounded"
              />
              {errors.email && <span className="text-red-500 text-sm">{errors.email.message}</span>}
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
            onClick={() => navigate('/organizacao/estipulantes')}
            className="px-6 py-2 bg-gray-300 rounded hover:bg-gray-400"
          >
            Cancelar
          </button>
        </div>
      </form>
    </div>
  )
}
