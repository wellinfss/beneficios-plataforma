namespace BeneficiosPlataforma.Domain.OrganizacaoHierarquica;

using Common;
using ValueObjects;

public class Estipulante : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string RazaoSocial { get; private set; } = null!;
    public string? NomeFantasia { get; private set; }
    public Cnpj Cnpj { get; private set; } = null!;
    public Endereco Endereco { get; private set; } = null!;
    public Telefone Telefone { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public Guid? GrupoEconomicoId { get; private set; }
    public string Status { get; private set; } = "ATIVO";

    public Estipulante() { }

    public Estipulante(
        Guid tenantId,
        string razaoSocial,
        string? nomeFantasia,
        Cnpj cnpj,
        Endereco endereco,
        Telefone telefone,
        Email email,
        Guid? grupoEconomicoId = null)
    {
        TenantId = tenantId;
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj;
        Endereco = endereco;
        Telefone = telefone;
        Email = email;
        GrupoEconomicoId = grupoEconomicoId;
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

    public void Atualizar(
        string razaoSocial,
        string? nomeFantasia,
        Endereco endereco,
        Telefone telefone,
        Email email,
        Guid? grupoEconomicoId = null)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Endereco = endereco;
        Telefone = telefone;
        Email = email;
        GrupoEconomicoId = grupoEconomicoId;
        UpdateTimestamp();
    }

    public void VincularGrupoEconomico(Guid grupoId)
    {
        GrupoEconomicoId = grupoId;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
