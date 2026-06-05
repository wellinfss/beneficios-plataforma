namespace BeneficiosPlataforma.Domain.ValueObjects;

using System.Text.RegularExpressions;
using Common;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email não pode ser vazio.");

        if (!IsValidEmail(value))
            throw new DomainException("Email inválido.");

        Value = value.ToLowerInvariant();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
