using Consultora.Application.Dtos;
using Consultora.Application.Validation;
using FluentValidation.TestHelper;

namespace Consultora.Tests.Validation;

public class ConsultorValidatorTests
{
    private readonly ConsultorCreateValidator _validator = new();

    [Theory]
    [InlineData(30)]
    [InlineData(200)]
    [InlineData(95.5)]
    public void TarifaHora_EnRango_EsValida(decimal tarifa)
    {
        var request = ValidRequest() with { TarifaHora = tarifa };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.TarifaHora);
    }

    [Theory]
    [InlineData(29)]
    [InlineData(201)]
    public void TarifaHora_FueraDeRango_EsInvalida(decimal tarifa)
    {
        var request = ValidRequest() with { TarifaHora = tarifa };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.TarifaHora).WithErrorMessage(
            "TarifaHora must be between 30 and 200 inclusive.");
    }

    [Fact]
    public void ProyectosActivos_MayorQueCinco_EsInvalido()
    {
        var request = ValidRequest() with { ProyectosActivos = 6 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProyectosActivos);
    }

    [Fact]
    public void ProyectosActivos_Negativo_EsInvalido()
    {
        var request = ValidRequest() with { ProyectosActivos = -1 };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProyectosActivos);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-an-email")]
    public void Email_Invalido_EsInvalido(string email)
    {
        var request = ValidRequest() with { Email = email };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Email_Valido_NoGeneraErrores()
    {
        var request = ValidRequest() with { Email = "ana.martinez@correo.test" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Request_CompletamenteValido_NoGeneraErrores()
    {
        var result = _validator.TestValidate(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    private static ConsultorCreateRequest ValidRequest() => new(
        NombreCompleto: "Ana Martinez Ponce",
        Email: "ana.martinez@correo.test",
        Area: "Estrategia",
        TarifaHora: 95m,
        ProyectosActivos: 3);
}