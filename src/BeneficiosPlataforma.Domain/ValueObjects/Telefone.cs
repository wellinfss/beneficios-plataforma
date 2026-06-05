namespace BeneficiosPlataforma.Domain.ValueObjects;

using Common;

public record Telefone
{
    public string Value { get; }

    public Telefone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Telefone não pode ser vazio.");

        var clean = value.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "");

        if (!clean.All(char.IsDigit))
            throw new DomainException("Telefone deve conter apenas dígitos.");

        if (clean.Length < 10 || clean.Length > 11)
            throw new DomainException("Telefone deve ter 10 ou 11 dígitos.");

        Value = clean;
    }
}
