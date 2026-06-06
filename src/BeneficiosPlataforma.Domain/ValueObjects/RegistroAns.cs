namespace BeneficiosPlataforma.Domain.ValueObjects;

using Common;

public record RegistroAns
{
    public string Value { get; }

    public RegistroAns(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Registro ANS não pode ser vazio.");

        var clean = value.Trim();

        if (clean.Length != 6 || !clean.All(char.IsDigit))
            throw new DomainException("Registro ANS deve conter exatamente 6 dígitos.");

        Value = clean;
    }
}
