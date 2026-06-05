namespace BeneficiosPlataforma.Application.Auth.Commands;

using FluentValidation;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain an uppercase letter")
            .Matches(@"[a-z]").WithMessage("Password must contain a lowercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain a digit");

        RuleFor(x => x.TenantSlug)
            .NotEmpty().WithMessage("Tenant slug is required");
    }
}
