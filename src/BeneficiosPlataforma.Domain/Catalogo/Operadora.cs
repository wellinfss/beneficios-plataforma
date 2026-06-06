namespace BeneficiosPlataforma.Domain.Catalogo;

using Common;
using ValueObjects;

public class Operadora : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string RazaoSocial { get; private set; } = null!;
    public Cnpj Cnpj { get; private set; } = null!;
    public RegistroAns? RegistroAns { get; private set; }
    public string Tipo { get; private set; } = null!;
    public string Status { get; private set; } = "ATIVO";
    public string? EndpointIntegracao { get; private set; }
    public string? FormatoIntegracao { get; private set; }
    public string? CredenciaisEncriptadas { get; private set; }

    public Operadora() { }

    public Operadora(Guid tenantId, string razaoSocial, Cnpj cnpj, string tipo, RegistroAns? registroAns = null)
    {
        TenantId = tenantId;
        RazaoSocial = razaoSocial;
        Cnpj = cnpj;
        Tipo = tipo;
        RegistroAns = registroAns;
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

    public void Atualizar(string razaoSocial, string tipo, RegistroAns? registroAns = null)
    {
        RazaoSocial = razaoSocial;
        Tipo = tipo;
        RegistroAns = registroAns;
        UpdateTimestamp();
    }

    public void AtualizarIntegracao(string? endpointIntegracao, string? formatoIntegracao, string? credenciaisEncriptadas)
    {
        EndpointIntegracao = endpointIntegracao;
        FormatoIntegracao = formatoIntegracao;
        if (credenciaisEncriptadas != null)
            CredenciaisEncriptadas = credenciaisEncriptadas;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
