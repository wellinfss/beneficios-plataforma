namespace BeneficiosPlataforma.Domain.Catalogo;

using Common;

public class Plano : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Nome { get; private set; } = null!;
    public Guid ProdutoId { get; private set; }
    public string? Cobertura { get; private set; }
    public string? AbrangenciaGeografica { get; private set; }
    public string? TipoAcomodacao { get; private set; }
    public decimal? ValorReferencia { get; private set; }
    public string Status { get; private set; } = "ATIVO";

    public Plano() { }

    public Plano(Guid tenantId, string nome, Guid produtoId, string? cobertura = null, string? abrangenciaGeografica = null, string? tipoAcomodacao = null, decimal? valorReferencia = null)
    {
        TenantId = tenantId;
        Nome = nome;
        ProdutoId = produtoId;
        Cobertura = cobertura;
        AbrangenciaGeografica = abrangenciaGeografica;
        TipoAcomodacao = tipoAcomodacao;
        ValorReferencia = valorReferencia;
        Status = "ATIVO";
    }

    public void Ativar()
    {
        Status = "ATIVO";
        UpdateTimestamp();
    }

    public void Inativar()
    {
        Status = "INATIVO";
        UpdateTimestamp();
    }

    public void Atualizar(string nome, string? cobertura = null, string? abrangenciaGeografica = null, string? tipoAcomodacao = null, decimal? valorReferencia = null)
    {
        Nome = nome;
        Cobertura = cobertura;
        AbrangenciaGeografica = abrangenciaGeografica;
        TipoAcomodacao = tipoAcomodacao;
        ValorReferencia = valorReferencia;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
