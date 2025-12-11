using EmployeeManagement.Application.DTOs;
using FluentValidation;

namespace EmployeeManagement.Application.Validators;

/// <summary>
/// Validador para requisições de atualização de funcionário.
/// </summary>
public class UpdateEmployeeRequestValidator : AbstractValidator<UpdateEmployeeRequest>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("O primeiro nome é obrigatório.")
            .MaximumLength(100).WithMessage("O primeiro nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("O sobrenome é obrigatório.")
            .MaximumLength(100).WithMessage("O sobrenome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.")
            .MaximumLength(255).WithMessage("O e-mail deve ter no máximo 255 caracteres.");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("A data de nascimento é obrigatória.")
            .Must(BeAtLeast18YearsOld).WithMessage("O funcionário deve ser maior de idade (18 anos ou mais).");

        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("O cargo informado não é válido.");

        RuleFor(x => x.Phones)
            .NotEmpty().WithMessage("É necessário informar pelo menos um telefone.")
            .Must(phones => phones.Count > 0).WithMessage("É necessário informar pelo menos um telefone.");

        RuleForEach(x => x.Phones).ChildRules(phone =>
        {
            phone.RuleFor(p => p.Number)
                .NotEmpty().WithMessage("O número de telefone é obrigatório.")
                .MaximumLength(20).WithMessage("O número de telefone deve ter no máximo 20 caracteres.");
        });
    }

    private static bool BeAtLeast18YearsOld(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        
        if (birthDate.Date > today.AddYears(-age))
            age--;

        return age >= 18;
    }
}

