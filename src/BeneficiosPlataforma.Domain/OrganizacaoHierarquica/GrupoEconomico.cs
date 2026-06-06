namespace BeneficiosPlataforma.Domain.OrganizacaoHierarquica;

using Common;
using ValueObjects;

public class GrupoEconomico : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Nome { get; private set; } = null!;
    public CnpjRaiz CnpjRaiz { get; private set; } = null!;
    public string Responsavel { get; private set; } = null!;
    public string Status { get; private set; } = "ATIVO";

    public GrupoEconomico() { }

    public GrupoEconomico(Guid tenantId, string nome, CnpjRaiz cnpjRaiz, string responsavel)
    {
        TenantId = tenantId;
        Nome = nome;
        CnpjRaiz = cnpjRaiz;
        Responsavel = responsavel;
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

    public void Atualizar(string nome, string responsavel)
    {
        Nome = nome;
        Responsavel = responsavel;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
