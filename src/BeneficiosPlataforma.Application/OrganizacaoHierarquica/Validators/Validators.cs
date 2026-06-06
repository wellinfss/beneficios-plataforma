namespace BeneficiosPlataforma.Application.OrganizacaoHierarquica.Validators;

using Commands;
using FluentValidation;

public class CriarGrupoEconomicoCommandValidator : AbstractValidator<CriarGrupoEconomicoCommand>
{
    public CriarGrupoEconomicoCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.CnpjRaiz)
            .NotEmpty().WithMessage("CNPJ Raiz é obrigatório")
            .Matches(@"^\d{8}$").WithMessage("CNPJ Raiz deve conter exatamente 8 dígitos");

        RuleFor(x => x.Responsavel)
            .NotEmpty().WithMessage("Responsável é obrigatório")
            .MaximumLength(255).WithMessage("Responsável deve ter no máximo 255 caracteres");
    }
}

public class AtualizarGrupoEconomicoCommandValidator : AbstractValidator<AtualizarGrupoEconomicoCommand>
{
    public AtualizarGrupoEconomicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(255).WithMessage("Nome deve ter no máximo 255 caracteres");

        RuleFor(x => x.Responsavel)
            .NotEmpty().WithMessage("Responsável é obrigatório")
            .MaximumLength(255).WithMessage("Responsável deve ter no máximo 255 caracteres");
    }
}

public class AlterarStatusGrupoEconomicoCommandValidator : AbstractValidator<AlterarStatusGrupoEconomicoCommand>
{
    public AlterarStatusGrupoEconomicoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}

public class CriarEstipulanteCommandValidator : AbstractValidator<CriarEstipulanteCommand>
{
    public CriarEstipulanteCommandValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Matches(@"^\d{14}$|^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$").WithMessage("CNPJ inválido");

        RuleFor(x => x.Endereco)
            .NotNull().WithMessage("Endereço é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Endereco.Logradouro)
                    .NotEmpty().WithMessage("Logradouro é obrigatório");
                RuleFor(x => x.Endereco.Numero)
                    .NotEmpty().WithMessage("Número é obrigatório");
                RuleFor(x => x.Endereco.Bairro)
                    .NotEmpty().WithMessage("Bairro é obrigatório");
                RuleFor(x => x.Endereco.Cidade)
                    .NotEmpty().WithMessage("Cidade é obrigatória");
                RuleFor(x => x.Endereco.Uf)
                    .NotEmpty().WithMessage("UF é obrigatório")
                    .Length(2).WithMessage("UF deve ter 2 caracteres");
                RuleFor(x => x.Endereco.Cep)
                    .NotEmpty().WithMessage("CEP é obrigatório")
                    .Matches(@"^\d+$").WithMessage("CEP deve conter apenas dígitos");
            });

        RuleFor(x => x.Telefone)
            .NotNull().WithMessage("Telefone é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Telefone.Numero)
                    .NotEmpty().WithMessage("Número do telefone é obrigatório");
            });

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Email.Endereco)
                    .NotEmpty().WithMessage("Email é obrigatório")
                    .EmailAddress().WithMessage("Email inválido");
            });
    }
}

public class AtualizarEstipulanteCommandValidator : AbstractValidator<AtualizarEstipulanteCommand>
{
    public AtualizarEstipulanteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");

        RuleFor(x => x.Endereco)
            .NotNull().WithMessage("Endereço é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Endereco.Logradouro)
                    .NotEmpty().WithMessage("Logradouro é obrigatório");
                RuleFor(x => x.Endereco.Numero)
                    .NotEmpty().WithMessage("Número é obrigatório");
                RuleFor(x => x.Endereco.Bairro)
                    .NotEmpty().WithMessage("Bairro é obrigatório");
                RuleFor(x => x.Endereco.Cidade)
                    .NotEmpty().WithMessage("Cidade é obrigatória");
                RuleFor(x => x.Endereco.Uf)
                    .NotEmpty().WithMessage("UF é obrigatório")
                    .Length(2).WithMessage("UF deve ter 2 caracteres");
                RuleFor(x => x.Endereco.Cep)
                    .NotEmpty().WithMessage("CEP é obrigatório")
                    .Matches(@"^\d+$").WithMessage("CEP deve conter apenas dígitos");
            });

        RuleFor(x => x.Telefone)
            .NotNull().WithMessage("Telefone é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Telefone.Numero)
                    .NotEmpty().WithMessage("Número do telefone é obrigatório");
            });

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email é obrigatório")
            .DependentRules(() =>
            {
                RuleFor(x => x.Email.Endereco)
                    .NotEmpty().WithMessage("Email é obrigatório")
                    .EmailAddress().WithMessage("Email inválido");
            });
    }
}

public class AlterarStatusEstipulanteCommandValidator : AbstractValidator<AlterarStatusEstipulanteCommand>
{
    public AlterarStatusEstipulanteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}

public class CriarSubestipulanteCommandValidator : AbstractValidator<CriarSubestipulanteCommand>
{
    public CriarSubestipulanteCommandValidator()
    {
        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("CNPJ é obrigatório")
            .Matches(@"^\d{14}$|^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$").WithMessage("CNPJ inválido");

        RuleFor(x => x.EstipulanteId)
            .NotEmpty().WithMessage("Estipulante é obrigatório");
    }
}

public class AtualizarSubestipulanteCommandValidator : AbstractValidator<AtualizarSubestipulanteCommand>
{
    public AtualizarSubestipulanteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .MaximumLength(255).WithMessage("Razão Social deve ter no máximo 255 caracteres");
    }
}

public class AlterarStatusSubestipulanteCommandValidator : AbstractValidator<AlterarStatusSubestipulanteCommand>
{
    public AlterarStatusSubestipulanteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id é obrigatório");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(x => x == "ATIVO" || x == "INATIVO").WithMessage("Status deve ser ATIVO ou INATIVO");
    }
}
