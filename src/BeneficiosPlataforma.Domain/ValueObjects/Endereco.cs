namespace BeneficiosPlataforma.Domain.ValueObjects;

public record Endereco(
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf,
    string Cep)
{
    public Endereco(string logradouro, string numero, string? complemento, string bairro, string cidade, string uf, string cep)
        : this(logradouro, numero, complemento, bairro, cidade, uf, cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new ArgumentException("Logradouro não pode ser vazio.", nameof(logradouro));
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("Número não pode ser vazio.", nameof(numero));
        if (string.IsNullOrWhiteSpace(bairro))
            throw new ArgumentException("Bairro não pode ser vazio.", nameof(bairro));
        if (string.IsNullOrWhiteSpace(cidade))
            throw new ArgumentException("Cidade não pode ser vazia.", nameof(cidade));
        if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
            throw new ArgumentException("UF deve ter 2 caracteres.", nameof(uf));
        if (string.IsNullOrWhiteSpace(cep) || !cep.All(char.IsDigit))
            throw new ArgumentException("CEP deve conter apenas dígitos.", nameof(cep));
    }
}
