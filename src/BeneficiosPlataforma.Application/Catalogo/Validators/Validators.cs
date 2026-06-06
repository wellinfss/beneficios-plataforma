namespace BeneficiosPlataforma.Application.Catalogo.Validators;

using Commands;
using FluentValidation;

public class CriarOperadoraCommandValidator : AbstractValidator<CriarOperadoraCommand>
{
    public CriarOperadoraCommandValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Matches(@"^\d{14}$|^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$").WithMessage("CNPJ inválido");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("Tipo é obrigatório")
            .Must(x => new[] { "SAUDE", "ODONTO", "VIDA", "OUTROS" }.Contains(x))
            .WithMessage("Tipo deve ser SAUDE, ODONTO, VIDA ou OUTROS");

        RuleFor(x => x.RegistroAns)
            .Matches(@"^\d{6}$").WithMessage("Registro ANS deve ter 6 dígitos")
            .When(x => !string.IsNullOrWhiteSpace(x.RegistroAns));
    }
}

public class AtualizarOperadoraCommandValidator : AbstractValidator<AtualizarOperadoraCommand>
{
    public AtualizarOperadoraCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");

        RuleFor(x => x.Tipo)
            .NotEmpty().WithMessage("Tipo é obrigatório")
            .Must(x => new[] { "SAUDE", "ODONTO", "VIDA", "OUTROS" }.Contains(x))
            .WithMessage("Tipo deve ser SAUDE, ODONTO, VIDA ou OUTROS");

        RuleFor(x => x.RegistroAns)
            .Matches(@"^\d{6}$").WithMessage("Registro ANS deve ter 6 dígitos")
            .When(x => !string.IsNullOrWhiteSpace(x.RegistroAns));
    }
}

public class AlterarStatusOperadoraCommandValidator : AbstractValidator<AlterarStatusOperadoraCommand>
{
    public AlterarStatusOperadoraCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}

public class AtualizarIntegracaoOperadoraCommandValidator : AbstractValidator<AtualizarIntegracaoOperadoraCommand>
{
    public AtualizarIntegracaoOperadoraCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.EndpointIntegracao)
            .Must(x => Uri.TryCreate(x, UriKind.Absolute, out _) || string.IsNullOrWhiteSpace(x))
            .WithMessage("Endpoint de Integração deve ser uma URL válida");
    }
}

public class CriarProdutoCommandValidator : AbstractValidator<CriarProdutoCommand>
{
    public CriarProdutoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.OperadoraId)
            .NotEmpty().WithMessage("Operadora é obrigatória");

        RuleFor(x => x.TipoBeneficio)
            .NotEmpty().WithMessage("Tipo de Benefício é obrigatório")
            .Must(x => new[] { "SAUDE", "ODONTO", "VIDA", "OUTROS" }.Contains(x))
            .WithMessage("Tipo de Benefício deve ser SAUDE, ODONTO, VIDA ou OUTROS");

        RuleFor(x => x.Modalidade)
            .NotEmpty().WithMessage("Modalidade é obrigatória")
            .Must(x => new[] { "COLETIVO_EMPRESARIAL", "POR_ADESAO" }.Contains(x))
            .WithMessage("Modalidade deve ser COLETIVO_EMPRESARIAL ou POR_ADESAO");
    }
}

public class AtualizarProdutoCommandValidator : AbstractValidator<AtualizarProdutoCommand>
{
    public AtualizarProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.TipoBeneficio)
            .NotEmpty().WithMessage("Tipo de Benefício é obrigatório")
            .Must(x => new[] { "SAUDE", "ODONTO", "VIDA", "OUTROS" }.Contains(x))
            .WithMessage("Tipo de Benefício deve ser SAUDE, ODONTO, VIDA ou OUTROS");

        RuleFor(x => x.Modalidade)
            .NotEmpty().WithMessage("Modalidade é obrigatória")
            .Must(x => new[] { "COLETIVO_EMPRESARIAL", "POR_ADESAO" }.Contains(x))
            .WithMessage("Modalidade deve ser COLETIVO_EMPRESARIAL ou POR_ADESAO");
    }
}

public class AlterarStatusProdutoCommandValidator : AbstractValidator<AlterarStatusProdutoCommand>
{
    public AlterarStatusProdutoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}

public class CriarPlanoCommandValidator : AbstractValidator<CriarPlanoCommand>
{
    public CriarPlanoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.ProdutoId)
            .NotEmpty().WithMessage("Produto é obrigatório");

        RuleFor(x => x.ValorReferencia)
            .GreaterThan(0).WithMessage("Valor de Referência deve ser maior que 0")
            .When(x => x.ValorReferencia.HasValue);
    }
}

public class AtualizarPlanoCommandValidator : AbstractValidator<AtualizarPlanoCommand>
{
    public AtualizarPlanoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.ValorReferencia)
            .GreaterThan(0).WithMessage("Valor de Referência deve ser maior que 0")
            .When(x => x.ValorReferencia.HasValue);
    }
}

public class AlterarStatusPlanoCommandValidator : AbstractValidator<AlterarStatusPlanoCommand>
{
    public AlterarStatusPlanoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}
