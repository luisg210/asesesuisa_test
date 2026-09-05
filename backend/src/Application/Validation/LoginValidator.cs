using Consultora.Application.Dtos;
using FluentValidation;

namespace Consultora.Application.Validation;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es obligatorio.")
            .EmailAddress().WithMessage("Email debe ser una dirección de correo válida.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password es obligatorio.");
    }
}