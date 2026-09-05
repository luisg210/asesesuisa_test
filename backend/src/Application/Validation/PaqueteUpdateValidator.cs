using Consultora.Application.Dtos;
using FluentValidation;

namespace Consultora.Application.Validation;

public class PaqueteUpdateValidator : AbstractValidator<PaqueteUpdateRequest>
{
    public PaqueteUpdateValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("Nombre es obligatorio.")
            .MaximumLength(120).WithMessage("Nombre no debe exceder los 120 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(500).WithMessage("Descripción no debe exceder los 500 caracteres.");

        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("Área es obligatoria.")
            .MaximumLength(80).WithMessage("Área no debe exceder los 80 caracteres.");

        RuleFor(x => x.Precio)
            .GreaterThanOrEqualTo(0).WithMessage("Precio debe ser mayor o igual a cero.");
    }
}