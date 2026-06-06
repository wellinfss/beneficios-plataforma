import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { catalogoApi } from '@/api/catalogoApi'
import type { OperadoraDto } from '@shared/catalogoTypes'

export function OperadorasPage() {
  const navigate = useNavigate()
  const [page, setPage] = useState(1)
  const [razaoSocial, setRazaoSocial] = useState('')
  const [tipo, setTipo] = useState('')
  const [status, setStatus] = useState('')

  const { data, isLoading, error } = useQuery({
    queryKey: ['operadoras', page, razaoSocial, tipo, status],
    queryFn: () =>
      catalogoApi.listarOperadoras({
        page,
        pageSize: 10,
        razaoSocial: razaoSocial || undefined,
        tipo: tipo || undefined,
        status: status || undefined,
      }),
  })

  const handleEdit = (id: string) => {
    navigate(`/catalogo/operadoras/${id}/editar`)
  }

  const handleDetail = (id: string) => {
    navigate(`/catalogo/operadoras/${id}`)
  }

  const handleDelete = async (id: string) => {
    if (confirm('Deseja excluir esta operadora?')) {
      try {
        await catalogoApi.excluirOperadora(id)
        window.location.reload()
      } catch (error) {
        alert('Erro ao excluir operadora')
      }
    }
  }

  const handleStatusChange = async (id: string, currentStatus: string) => {
    const newStatus = currentStatus === 'ATIVO' ? 'INATIVO' : 'ATIVO'
    try {
      await catalogoApi.alterarStatusOperadora(id, newStatus as any)
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
        <h1 className="text-3xl font-bold">Operadoras</h1>
        <button
          onClick={() => navigate('/catalogo/operadoras/novo')}
          className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
        >
          Nova Operadora
        </button>
      </div>

      <div className="mb-6 flex gap-4">
        <input
          type="text"
          placeholder="Buscar por razão social..."
          value={razaoSocial}
          onChange={(e) => {
            setRazaoSocial(e.target.value)
            setPage(1)
          }}
          className="flex-1 px-4 py-2 border rounded"
        />
        <select
          value={tipo}
          onChange={(e) => {
            setTipo(e.target.value)
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
              <th className="border p-2 text-left">Razão Social</th>
              <th className="border p-2 text-left">CNPJ</th>
              <th className="border p-2 text-left">ANS</th>
              <th className="border p-2 text-left">Tipo</th>
              <th className="border p-2 text-left">Status</th>
              <th className="border p-2 text-left">Ações</th>
            </tr>
          </thead>
          <tbody>
            {data?.items.map((operadora: OperadoraDto) => (
              <tr key={operadora.id} className="hover:bg-gray-50">
                <td className="border p-2">{operadora.razaoSocial}</td>
                <td className="border p-2">{operadora.cnpj}</td>
                <td className="border p-2">{operadora.registroAns || '-'}</td>
                <td className="border p-2">{operadora.tipo}</td>
                <td className="border p-2">
                  <button
                    onClick={() => handleStatusChange(operadora.id, operadora.status)}
                    className={`px-3 py-1 rounded text-white ${
                      operadora.status === 'ATIVO'
                        ? 'bg-green-500 hover:bg-green-600'
                        : 'bg-red-500 hover:bg-red-600'
                    }`}
                  >
                    {operadora.status}
                  </button>
                </td>
                <td className="border p-2 space-x-2">
                  <button
                    onClick={() => handleDetail(operadora.id)}
                    className="bg-blue-500 text-white px-2 py-1 rounded text-sm hover:bg-blue-600"
                  >
                    Detalhe
                  </button>
                  <button
                    onClick={() => handleEdit(operadora.id)}
                    className="bg-yellow-500 text-white px-2 py-1 rounded text-sm hover:bg-yellow-600"
                  >
                    Editar
                  </button>
                  <button
                    onClick={() => handleDelete(operadora.id)}
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
