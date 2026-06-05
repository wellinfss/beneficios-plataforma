namespace BeneficiosPlataforma.Domain.ValueObjects;

using Common;

public record Cnpj
{
    public string Value { get; }

    public Cnpj(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("CNPJ não pode ser vazio.");

        var clean = value.Replace(".", "").Replace("-", "").Replace("/", "");

        if (clean.Length != 14 || !clean.All(char.IsDigit))
            throw new DomainException("CNPJ deve conter exatamente 14 dígitos.");

        if (!ValidateCnpj(clean))
            throw new DomainException("CNPJ inválido.");

        Value = clean;
    }

    private static bool ValidateCnpj(string cnpj)
    {
        if (cnpj.All(c => c == cnpj[0]))
            return false;

        var digits = cnpj.Select(c => int.Parse(c.ToString())).ToArray();

        var sum = 0;
        var multiplier = 5;
        for (int i = 0; i < 12; i++)
        {
            sum += digits[i] * multiplier;
            multiplier = multiplier == 2 ? 9 : multiplier - 1;
        }

        var firstDigit = (sum % 11) switch
        {
            < 2 => 0,
            _ => 11 - (sum % 11)
        };

        if (digits[12] != firstDigit)
            return false;

        sum = 0;
        multiplier = 6;
        for (int i = 0; i < 13; i++)
        {
            sum += digits[i] * multiplier;
            multiplier = multiplier == 2 ? 9 : multiplier - 1;
        }

        var secondDigit = (sum % 11) switch
        {
            < 2 => 0,
            _ => 11 - (sum % 11)
        };

        return digits[13] == secondDigit;
    }
}
