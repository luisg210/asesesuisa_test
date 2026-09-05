using Consultora.Application.Dtos;
using FluentValidation;

namespace Consultora.Application.Validation;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken es obligatorio.");
    }
}