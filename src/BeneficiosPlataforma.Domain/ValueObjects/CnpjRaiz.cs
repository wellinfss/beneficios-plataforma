namespace BeneficiosPlataforma.Domain.ValueObjects;

using Common;

public record CnpjRaiz
{
    public string Value { get; }

    public CnpjRaiz(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CNPJ Raiz não pode ser vazio.");

        var clean = value.Replace(".", "").Replace("-", "").Replace("/", "");

        if (clean.Length != 8 || !clean.All(char.IsDigit))
            throw new DomainException("CNPJ Raiz deve conter exatamente 8 dígitos.");

        Value = clean;
    }
}
