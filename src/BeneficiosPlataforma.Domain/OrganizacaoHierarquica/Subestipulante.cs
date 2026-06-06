namespace BeneficiosPlataforma.Domain.OrganizacaoHierarquica;

using Common;
using ValueObjects;

public class Subestipulante : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string RazaoSocial { get; private set; } = null!;
    public string? NomeFantasia { get; private set; }
    public Cnpj Cnpj { get; private set; } = null!;
    public Endereco? Endereco { get; private set; }
    public Telefone? Telefone { get; private set; }
    public Email? Email { get; private set; }
    public Guid EstipulanteId { get; private set; }
    public string Status { get; private set; } = "ATIVO";

    public Subestipulante() { }

    public Subestipulante(
        Guid tenantId,
        string razaoSocial,
        string? nomeFantasia,
        Cnpj cnpj,
        Guid estipulanteId,
        Endereco? endereco = null,
        Telefone? telefone = null,
        Email? email = null)
    {
        TenantId = tenantId;
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Cnpj = cnpj;
        EstipulanteId = estipulanteId;
        Endereco = endereco;
        Telefone = telefone;
        Email = email;
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
        Endereco? endereco = null,
        Telefone? telefone = null,
        Email? email = null)
    {
        RazaoSocial = razaoSocial;
        NomeFantasia = nomeFantasia;
        Endereco = endereco;
        Telefone = telefone;
        Email = email;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
