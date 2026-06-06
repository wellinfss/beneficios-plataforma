import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { organizacaoApi } from '@/api/organizacaoApi'
import type { EstipulanteDto } from '@shared/organizacaoTypes'

export function EstipulantesPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [nome, setNome] = useState('')
  const [status, setStatus] = useState('')

  const { data, isLoading, error } = useQuery({
    queryKey: ['estipulantes', page, nome, status],
    queryFn: () =>
      organizacaoApi.listarEstipulantes({
        page,
        pageSize: 10,
        nome: nome || undefined,
        status: status || undefined,
      }),
  })

  const handleEdit = (id: string) => {
    navigate(`/organizacao/estipulantes/${id}/editar`)
  }

  const handleDetail = (id: string) => {
    navigate(`/organizacao/estipulantes/${id}`)
  }

  const handleDelete = async (id: string) => {
    if (confirm('Deseja excluir este estipulante?')) {
      try {
        await organizacaoApi.excluirEstipulante(id)
        window.location.reload()
      } catch (error) {
        alert('Erro ao excluir estipulante')
      }
    }
  }

  const handleStatusChange = async (id: string, currentStatus: string) => {
    const newStatus = currentStatus === 'ATIVO' ? 'INATIVO' : 'ATIVO'
    try {
      await organizacaoApi.alterarStatusEstipulante(id, newStatus as any)
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
        <h1 className="text-3xl font-bold">Estipulantes</h1>
        <button
          onClick={() => navigate('/organizacao/estipulantes/novo')}
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          Novo Estipulante
        </button>
      </div>

      <div className="mb-6 flex gap-4">
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
              <th className="border p-2 text-left">Razão Social</th>
              <th className="border p-2 text-left">CNPJ</th>
              <th className="border p-2 text-left">Status</th>
              <th className="border p-2 text-left">Ações</th>
            </tr>
          </thead>
          <tbody>
            {data?.items.map((estipulante: EstipulanteDto) => (
              <tr key={estipulante.id} className="hover:bg-gray-50">
                <td className="border p-2">{estipulante.razaoSocial}</td>
                <td className="border p-2">{estipulante.cnpj}</td>
                <td className="border p-2">
                  <span
                    className={`px-2 py-1 rounded text-sm ${
                      estipulante.status === 'ATIVO'
                        ? 'bg-green-100 text-green-800'
                        : 'bg-red-100 text-red-800'
                    }`}
                  >
                    {estipulante.status}
                  </span>
                </td>
                <td className="border p-2 flex gap-2">
                  <button
                    onClick={() => handleDetail(estipulante.id)}
                    className="bg-green-500 text-white px-3 py-1 rounded text-sm hover:bg-green-600"
                  >
                    Detalhe
                  </button>
                  <button
                    onClick={() => handleEdit(estipulante.id)}
                    className="bg-blue-500 text-white px-3 py-1 rounded text-sm hover:bg-blue-600"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleStatusChange(estipulante.id, estipulante.status)}
                    className="bg-yellow-500 text-white px-3 py-1 rounded text-sm hover:bg-yellow-600"
                  >
                    {estipulante.status === 'ATIVO' ? 'Inativar' : 'Ativar'}
                  </button>
                  <button
                    onClick={() => handleDelete(estipulante.id)}
                    className="bg-red-500 text-white px-3 py-1 rounded text-sm hover:bg-red-600"
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
        <button
          onClick={() => setPage(Math.max(1, page - 1))}
          disabled={!data?.hasPreviousPage}
          className="px-4 py-2 bg-gray-300 rounded disabled:opacity-50"
        >
          Anterior
        </button>
        <span>
          Página {page} de {data?.totalPages || 1}
        </span>
        <button
          onClick={() => setPage(page + 1)}
          disabled={!data?.hasNextPage}
          className="px-4 py-2 bg-gray-300 rounded disabled:opacity-50"
        >
          Próxima
        </button>
      </div>
    </div>
  )
}
