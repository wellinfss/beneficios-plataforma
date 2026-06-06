namespace BeneficiosPlataforma.Application.Catalogo;

public class OperadoraDto
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; } = null!;
    public string Cnpj { get; set; } = null!;
    public string? RegistroAns { get; set; }
    public string Tipo { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? EndpointIntegracao { get; set; }
    public string? FormatoIntegracao { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProdutoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public Guid OperadoraId { get; set; }
    public string? OperadoraNome { get; set; }
    public string TipoBeneficio { get; set; } = null!;
    public string Modalidade { get; set; } = null!;
    public string? RegistroAnsProduto { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PlanoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public Guid ProdutoId { get; set; }
    public string? ProdutoNome { get; set; }
    public Guid? OperadoraId { get; set; }
    public string? OperadoraNome { get; set; }
    public string? Cobertura { get; set; }
    public string? AbrangenciaGeografica { get; set; }
    public string? TipoAcomodacao { get; set; }
    public decimal? ValorReferencia { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class IntegracaoOperadoraDto
{
    public string? EndpointIntegracao { get; set; }
    public string? FormatoIntegracao { get; set; }
    public string? Credenciais { get; set; }
}
