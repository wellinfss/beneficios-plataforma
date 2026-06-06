namespace BeneficiosPlataforma.Domain.Catalogo;

using Common;

public class Produto : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; private set; }
    public string Nome { get; private set; } = null!;
    public Guid OperadoraId { get; private set; }
    public string TipoBeneficio { get; private set; } = null!;
    public string Modalidade { get; private set; } = null!;
    public string? RegistroAnsProduto { get; private set; }
    public string Status { get; private set; } = "ATIVO";

    public Produto() { }

    public Produto(Guid tenantId, string nome, Guid operadoraId, string tipoBeneficio, string modalidade, string? registroAnsProduto = null)
    {
        TenantId = tenantId;
        Nome = nome;
        OperadoraId = operadoraId;
        TipoBeneficio = tipoBeneficio;
        Modalidade = modalidade;
        RegistroAnsProduto = registroAnsProduto;
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

    public void Atualizar(string nome, string tipoBeneficio, string modalidade, string? registroAnsProduto = null)
    {
        Nome = nome;
        TipoBeneficio = tipoBeneficio;
        Modalidade = modalidade;
        RegistroAnsProduto = registroAnsProduto;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        Inativar();
        IsDeleted = true;
    }
}
