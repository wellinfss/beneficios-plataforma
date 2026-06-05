namespace BeneficiosPlataforma.Infrastructure.Messaging;

using BeneficiosPlataforma.Domain.Events;
using System.Collections.Immutable;

public static class EventTypeRegistry
{
    private static readonly ImmutableDictionary<string, Type> TypeMap = new Dictionary<string, Type>
    {
        { "mdm.pessoas.criada", typeof(PessoaCriadaEvent) },
        { "mdm.pessoas.atualizada", typeof(PessoaAtualizadaEvent) },
        { "mdm.produtos.criado", typeof(ProdutoCriadoEvent) },
        { "mdm.produtos.atualizado", typeof(ProdutoAtualizadoEvent) },
        { "mdm.empresas.criada", typeof(EmpresaCriadaEvent) },
        { "mdm.empresas.atualizada", typeof(EmpresaAtualizadaEvent) },
    }.ToImmutableDictionary();

    public static Type? GetEventType(string eventTypeString)
    {
        TypeMap.TryGetValue(eventTypeString, out var type);
        return type;
    }
}
