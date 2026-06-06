import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { catalogoApi } from '@/api/catalogoApi'
import type { PlanoDto, OperadoraDto } from '@shared/catalogoTypes'

export function PlanosPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [nome, setNome] = useState('')
  const [operadoraId, setOperadoraId] = useState('')
  const [tipoBeneficio, setTipoBeneficio] = useState('')
  const [status, setStatus] = useState('')

  const { data: operadoras } = useQuery({
    queryKey: ['operadoras'],
    queryFn: () =>
      catalogoApi.listarOperadoras({
        pageSize: 1000,
      }),
  })

  const { data, isLoading, error } = useQuery({
    queryKey: ['planos', page, nome, operadoraId, tipoBeneficio, status],
    queryFn: () =>
      catalogoApi.listarPlanos({
        page,
        pageSize: 10,
        nome: nome || undefined,
        operadoraId: operadoraId || undefined,
        tipoBeneficio: tipoBeneficio || undefined,
        status: status || undefined,
      }),
  })

  const handleEdit = (id: string) => {
    navigate(`/catalogo/planos/${id}/editar`)
  }

  const handleDelete = async (id: string) => {
    if (confirm('Deseja excluir este plano?')) {
      try {
        await catalogoApi.excluirPlano(id)
        window.location.reload()
      } catch (error) {
        alert('Erro ao excluir plano')
      }
    }
  }

  const handleStatusChange = async (id: string, currentStatus: string) => {
    const newStatus = currentStatus === 'ATIVO' ? 'INATIVO' : 'ATIVO'
    try {
      await catalogoApi.alterarStatusPlano(id, newStatus as any)
      window.location.reload()
    } catch (error) {
      alert('Erro ao alterar status')
    }
  }

  if (isLoading) return <div>Carregando...</div>
  if (error) return <div>Erro ao carregar dados</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">Planos</h1>
        <button
          onClick={() => navigate('/catalogo/planos/novo')}
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          Novo Plano
        </button>
      </div>

      <div className="mb-6 flex gap-4 flex-wrap">
        <input
          type="text"
          placeholder="Buscar por nome..."
          value={nome}
          onChange={(e) => {
            setNome(e.target.value)
            setPage(1)
          }}
          className="flex-1 px-4 py-2 border rounded"
        />
        <select
          value={operadoraId}
          onChange={(e) => {
            setOperadoraId(e.target.value)
            setPage(1)
          }}
          className="px-4 py-2 border rounded"
        >
          <option value="">Todas as Operadoras</option>
          {operadoras?.items.map((op: OperadoraDto) => (
            <option key={op.id} value={op.id}>
              {op.razaoSocial}
            </option>
          ))}
        </select>
        <select
          value={tipoBeneficio}
          onChange={(e) => {
            setTipoBeneficio(e.target.value)
            setPage(1)
          }}
          className="px-4 py-2 border rounded"
        >
          <option value="">Todos os Tipos</option>
          <option value="SAUDE">Saúde</option>
          <option value="ODONTO">Odontologia</option>
          <option value="VIDA">Vida</option>
          <option value="OUTROS">Outros</option>
        </select>
        <select
          value={status}
          onChange={(e) => {
            setStatus(e.target.value)
            setPage(1)
          }}
          className="px-4 py-2 border rounded"
        >
          <option value="">Todos os Status</option>
          <option value="ATIVO">Ativo</option>
          <option value="INATIVO">Inativo</option>
        </select>
      </div>

      <div className="overflow-x-auto">
        <table className="w-full border-collapse border border-gray-300">
          <thead className="bg-gray-100">
            <tr>
              <th className="border p-2 text-left">Nome</th>
              <th className="border p-2 text-left">Produto</th>
              <th className="border p-2 text-left">Operadora</th>
              <th className="border p-2 text-left">Status</th>
              <th className="border p-2 text-left">Ações</th>
            </tr>
          </thead>
          <tbody>
            {data?.items.map((plano: PlanoDto) => (
              <tr key={plano.id} className="hover:bg-gray-50">
                <td className="border p-2">{plano.nome}</td>
                <td className="border p-2">{plano.produtoNome || '-'}</td>
                <td className="border p-2">{plano.operadoraNome || '-'}</td>
                <td className="border p-2">
                  <button
                    onClick={() => handleStatusChange(plano.id, plano.status)}
                    className={`px-3 py-1 rounded text-white ${
                      plano.status === 'ATIVO'
                        ? 'bg-green-500 hover:bg-green-600'
                        : 'bg-red-500 hover:bg-red-600'
                    }`}
                  >
                    {plano.status}
                  </button>
                </td>
                <td className="border p-2 space-x-2">
                  <button
                    onClick={() => handleEdit(plano.id)}
                    className="bg-yellow-500 text-white px-2 py-1 rounded text-sm hover:bg-yellow-600"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(plano.id)}
                    className="bg-red-500 text-white px-2 py-1 rounded text-sm hover:bg-red-600"
                  >
                    Excluir
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className="mt-6 flex justify-between items-center">
        <div>
          Total: {data?.totalCount} | Página {data?.pageNumber} de {data?.totalPages}
        </div>
        <div className="space-x-2">
          <button
            disabled={!data?.hasPreviousPage}
            onClick={() => setPage(page - 1)}
            className="px-4 py-2 border rounded disabled:opacity-50"
          >
            Anterior
          </button>
          <button
            disabled={!data?.hasNextPage}
            onClick={() => setPage(page + 1)}
            className="px-4 py-2 border rounded disabled:opacity-50"
          >
            Próxima
          </button>
        </div>
      </div>
    </div>
  )
}
