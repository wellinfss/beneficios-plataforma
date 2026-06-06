import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { organizacaoApi } from '@/api/organizacaoApi'
import type { SubestipulanteDto } from '@shared/organizacaoTypes'

export function EstipulanteDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const { data: hierarquia, isLoading, error } = useQuery({
    queryKey: ['estipulantes-hierarquia', id],
    queryFn: () => organizacaoApi.obterHierarquiaEstipulante(id!),
    enabled: Boolean(id),
  })

  if (isLoading) return <div>Carregando...</div>
  if (error) return <div>Erro ao carregar dados</div>
  if (!hierarquia) return <div>Estipulante não encontrado</div>

  const { estipulante, subestipulantes } = hierarquia

  return (
    <div className="p-6">
      <button
        onClick={() => navigate('/organizacao/estipulantes')}
        className="mb-4 px-4 py-2 bg-gray-300 rounded hover:bg-gray-400"
      >
        ← Voltar
      </button>

      {estipulante.grupoEconomicoNome && (
        <div className="mb-4 text-sm text-gray-600">
          <span className="cursor-pointer hover:text-blue-600" onClick={() => navigate('/organizacao/grupos-economicos')}>
            Grupos Econômicos
          </span>
          {' > '}
          <span className="font-semibold">{estipulante.grupoEconomicoNome}</span>
          {' > '}
          <span className="font-semibold">{estipulante.razaoSocial}</span>
        </div>
      )}

      <div className="bg-white p-6 rounded border mb-6">
        <h1 className="text-3xl font-bold mb-4">{estipulante.razaoSocial}</h1>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <p className="text-sm text-gray-600">CNPJ</p>
            <p className="text-lg font-semibold">{estipulante.cnpj}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Status</p>
            <span
              className={`px-2 py-1 rounded text-sm ${
                estipulante.status === 'ATIVO'
                  ? 'bg-green-100 text-green-800'
                  : 'bg-red-100 text-red-800'
              }`}
            >
              {estipulante.status}
            </span>
          </div>
          {estipulante.nomeFantasia && (
            <div>
              <p className="text-sm text-gray-600">Nome Fantasia</p>
              <p className="text-lg">{estipulante.nomeFantasia}</p>
            </div>
          )}
          <div>
            <p className="text-sm text-gray-600">Email</p>
            <p className="text-lg">{estipulante.email.endereco}</p>
          </div>
          <div>
            <p className="text-sm text-gray-600">Telefone</p>
            <p className="text-lg">{estipulante.telefone.numero}</p>
          </div>
        </div>
      </div>

      <div>
        <h2 className="text-2xl font-bold mb-4">Subestipulantes</h2>
        {subestipulantes.length === 0 ? (
          <p className="text-gray-500">Nenhum subestipulante vinculado</p>
        ) : (
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
                {subestipulantes.map((sub: SubestipulanteDto) => (
                  <tr key={sub.id} className="hover:bg-gray-50">
                    <td className="border p-2">{sub.razaoSocial}</td>
                    <td className="border p-2">{sub.cnpj}</td>
                    <td className="border p-2">
                      <span
                        className={`px-2 py-1 rounded text-sm ${
                          sub.status === 'ATIVO'
                            ? 'bg-green-100 text-green-800'
                            : 'bg-red-100 text-red-800'
                        }`}
                      >
                        {sub.status}
                      </span>
                    </td>
                    <td className="border p-2 flex gap-2">
                      <button
                        onClick={() => navigate(`/organizacao/subestipulantes/${sub.id}/editar`)}
                        className="bg-blue-500 text-white px-3 py-1 rounded text-sm hover:bg-blue-600"
                      >
                        Editar
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  )
}
