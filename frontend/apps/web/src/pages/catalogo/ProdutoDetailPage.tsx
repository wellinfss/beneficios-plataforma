import { useParams, useNavigate } from 'react-router-dom'
import { useQuery } from '@tanstack/react-query'
import { catalogoApi } from '@/api/catalogoApi'

export function ProdutoDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()

  const { data: produto, isLoading } = useQuery({
    queryKey: ['produtos', id],
    queryFn: () => catalogoApi.obterProduto(id!),
  })

  const { data: planos } = useQuery({
    queryKey: ['produto-planos', id],
    queryFn: () => catalogoApi.listarPlanosPorProduto(id!),
  })

  if (isLoading) return <div>Carregando...</div>
  if (!produto) return <div>Produto não encontrado</div>

  return (
    <div className="p-6">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold">{produto.nome}</h1>
        <button
          onClick={() => navigate('/catalogo/produtos')}
          className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
        >
          Voltar
        </button>
      </div>

      <div className="grid grid-cols-2 gap-4 mb-8 bg-gray-50 p-4 rounded">
        <div>
          <label className="font-semibold">Operadora:</label>
          <p>{produto.operadoraNome || '-'}</p>
        </div>
        <div>
          <label className="font-semibold">Tipo de Benefício:</label>
          <p>{produto.tipoBeneficio}</p>
        </div>
        <div>
          <label className="font-semibold">Modalidade:</label>
          <p>{produto.modalidade}</p>
        </div>
        <div>
          <label className="font-semibold">Status:</label>
          <p className={`px-2 py-1 rounded text-white w-fit ${
            produto.status === 'ATIVO' ? 'bg-green-500' : 'bg-red-500'
          }`}>
            {produto.status}
          </p>
        </div>
        {produto.registroAnsProduto && (
          <div>
            <label className="font-semibold">Registro ANS:</label>
            <p>{produto.registroAnsProduto}</p>
          </div>
        )}
      </div>

      <h2 className="text-2xl font-bold mb-4">Planos</h2>
      {planos && planos.length > 0 ? (
        <div className="space-y-2">
          {planos.map((plano) => (
            <div key={plano.id} className="border p-4 rounded hover:bg-gray-50">
              <div className="flex justify-between items-center">
                <div>
                  <p className="font-semibold">{plano.nome}</p>
                  {plano.cobertura && <p className="text-sm text-gray-600">Cobertura: {plano.cobertura}</p>}
                  {plano.valorReferencia && <p className="text-sm text-gray-600">Valor: R$ {plano.valorReferencia.toFixed(2)}</p>}
                </div>
                <button
                  onClick={() => navigate(`/catalogo/planos/${plano.id}/editar`)}
                  className="bg-blue-500 text-white px-3 py-1 rounded text-sm hover:bg-blue-600"
                >
                  Editar
                </button>
              </div>
            </div>
          ))}
        </div>
      ) : (
        <p className="text-gray-600">Nenhum plano vinculado</p>
      )}
    </div>
  )
}
