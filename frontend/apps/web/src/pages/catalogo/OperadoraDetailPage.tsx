import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { catalogoApi } from '@/api/catalogoApi'

export function OperadoraDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const { data: operadora, isLoading } = useQuery({
    queryKey: ['operadoras', id],
    queryFn: () => catalogoApi.obterOperadora(id!),
  })

  const { data: produtos } = useQuery({
    queryKey: ['operadora-produtos', id],
    queryFn: () => catalogoApi.listarProdutosPorOperadora(id!),
  })

  if (isLoading) return <div>Carregando...</div>
  if (!operadora) return <div>Operadora não encontrada</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">{operadora.razaoSocial}</h1>
        <button
          onClick={() => navigate('/catalogo/operadoras')}
          className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
        >
          Voltar
        </button>
      </div>

      <div className="grid grid-cols-2 gap-4 mb-8 bg-gray-50 p-4 rounded">
        <div>
          <label className="font-semibold">CNPJ:</label>
          <p>{operadora.cnpj}</p>
        </div>
        <div>
          <label className="font-semibold">Registro ANS:</label>
          <p>{operadora.registroAns || '-'}</p>
        </div>
        <div>
          <label className="font-semibold">Tipo:</label>
          <p>{operadora.tipo}</p>
        </div>
        <div>
          <label className="font-semibold">Status:</label>
          <p className={`px-2 py-1 rounded text-white w-fit ${
            operadora.status === 'ATIVO' ? 'bg-green-500' : 'bg-red-500'
          }`}>
            {operadora.status}
          </p>
        </div>
        {operadora.endpointIntegracao && (
          <div className="col-span-2">
            <label className="font-semibold">Endpoint de Integração:</label>
            <p>{operadora.endpointIntegracao}</p>
          </div>
        )}
      </div>

      <h2 className="text-2xl font-bold mb-4">Produtos</h2>
      {produtos && produtos.length > 0 ? (
        <div className="space-y-2">
          {produtos.map((produto) => (
            <div key={produto.id} className="border p-4 rounded hover:bg-gray-50">
              <div className="flex justify-between items-center">
                <div>
                  <p className="font-semibold">{produto.nome}</p>
                  <p className="text-sm text-gray-600">{produto.tipoBeneficio} - {produto.modalidade}</p>
                </div>
                <button
                  onClick={() => navigate(`/catalogo/produtos/${produto.id}/editar`)}
                  className="bg-blue-500 text-white px-3 py-1 rounded text-sm hover:bg-blue-600"
                >
                  Editar
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p className="text-gray-600">Nenhum produto vinculado</p>
      )}
    </div>
  )
}
