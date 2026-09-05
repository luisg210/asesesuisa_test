using Consultora.Application.Dtos;
using FluentValidation;

namespace Consultora.Application.Validation;

public class ConsultorCreateValidator : AbstractValidator<ConsultorCreateRequest>
{
    public ConsultorCreateValidator()
    {
        RuleFor(x => x.NombreCompleto)
            .NotEmpty().WithMessage("NombreCompleto es obligatorio.")
            .MaximumLength(150).WithMessage("NombreCompleto no debe exceder los 150 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email es obligatorio.")
            .EmailAddress().WithMessage("Email debe ser una dirección de correo válida.")
            .MaximumLength(150).WithMessage("Email no debe exceder los 150 caracteres.");

        RuleFor(x => x.Area)
            .NotEmpty().WithMessage("Área es obligatoria.")
            .MaximumLength(80).WithMessage("Área no debe exceder los 80 caracteres.");

        RuleFor(x => x.TarifaHora)
            .InclusiveBetween(30, 200).WithMessage("TarifaHora debe estar entre 30 y 200 inclusive.");

        RuleFor(x => x.ProyectosActivos)
            .InclusiveBetween(0, 5).WithMessage("ProyectosActivos debe estar entre 0 y 5.");
    }
}