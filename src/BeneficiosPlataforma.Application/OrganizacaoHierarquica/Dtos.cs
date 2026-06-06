namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica;

using Common;

public class GrupoEconomicoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = null!;
    public string CnpjRaiz { get; set; } = null!;
    public string Responsavel { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class EstipulanteDto
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = null!;
    public EnderecoDto Endereco { get; set; } = null!;
    public TelefoneDto Telefone { get; set; } = null!;
    public EmailDto Email { get; set; } = null!;
    public Guid? GrupoEconomicoId { get; set; }
    public string? GrupoEconomicoNome { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SubestipulanteDto
{
    public Guid Id { get; set; }
    public string RazaoSocial { get; set; } = null!;
    public string? NomeFantasia { get; set; }
    public string Cnpj { get; set; } = null!;
    public EnderecoDto? Endereco { get; set; }
    public TelefoneDto? Telefone { get; set; }
    public EmailDto? Email { get; set; }
    public Guid EstipulanteId { get; set; }
    public string? EstipulanteRazaoSocial { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class EnderecoDto
{
    public string Logradouro { get; set; } = null!;
    public string Numero { get; set; } = null!;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = null!;
    public string Cidade { get; set; } = null!;
    public string Uf { get; set; } = null!;
    public string Cep { get; set; } = null!;
}

public class TelefoneDto
{
    public string Numero { get; set; } = null!;
}

public class EmailDto
{
    public string Endereco { get; set; } = null!;
}

public class ListarGruposEconomicosQuery : IRequest<PagedResult<GrupoEconomicoDto>>
{
    public string? Nome { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
