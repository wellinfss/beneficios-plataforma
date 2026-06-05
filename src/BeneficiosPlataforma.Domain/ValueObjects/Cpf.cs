namespace BeneficiosPlataforma.Domain.ValueObjects;

using Common;

public record Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CPF não pode ser vazio.");

        var clean = value.Replace(".", "").Replace("-", "");

        if (clean.Length != 11 || !clean.All(char.IsDigit))
            throw new DomainException("CPF deve conter exatamente 11 dígitos.");

        if (!ValidateCpf(clean))
            throw new DomainException("CPF inválido.");

        Value = clean;
    }

    private static bool ValidateCpf(string cpf)
    {
        if (cpf.All(c => c == cpf[0]))
            return false;

        var digits = cpf.Select(c => int.Parse(c.ToString())).ToArray();

        var sum = 0;
        for (int i = 0; i < 9; i++)
            sum += digits[i] * (10 - i);

        var firstDigit = (sum % 11) switch
        {
            < 2 => 0,
            _ => 11 - (sum % 11)
        };

        if (digits[9] != firstDigit)
            return false;

        sum = 0;
        for (int i = 0; i < 10; i++)
            sum += digits[i] * (11 - i);

        var secondDigit = (sum % 11) switch
        {
            < 2 => 0,
            _ => 11 - (sum % 11)
        };

        return digits[10] == secondDigit;
    }
}
