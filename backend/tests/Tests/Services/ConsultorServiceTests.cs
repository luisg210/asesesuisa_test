using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Services;
using Consultora.Application.Validation;
using Consultora.Domain.Entities;
using Consultora.Tests.Fakes;
using FluentValidation;

namespace Consultora.Tests.Services;

public class ConsultorServiceTests
{
    private static ConsultorService CreateService(FakeConsultorRepository repository)
        => new(repository, new ConsultorCreateValidator(), new ConsultorUpdateValidator());

    [Fact]
    public async Task CreateAsync_NombreYAreaDuplicados_LanzaConflict()
    {
        var repository = new FakeConsultorRepository();
        await repository.InsertAsync(new Consultor
        {
            NombreCompleto = "Ana Martinez Ponce",
            Email = "otro@correo.test",
            Area = "Estrategia",
            TarifaHora = 100m,
            ProyectosActivos = 1
        });

        var service = CreateService(repository);
        var request = new ConsultorCreateRequest(
            "Ana Martinez Ponce", "ana.martinez@correo.test", "Estrategia", 95m, 2);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
        Assert.Equal("A consultant with the same name and area already exists.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_EmailDuplicado_LanzaConflict()
    {
        var repository = new FakeConsultorRepository();
        await repository.InsertAsync(new Consultor
        {
            NombreCompleto = "Otra Persona",
            Email = "ana.martinez@correo.test",
            Area = "Finanzas",
            TarifaHora = 100m,
            ProyectosActivos = 1
        });

        var service = CreateService(repository);
        var request = new ConsultorCreateRequest(
            "Ana Martinez Ponce", "ana.martinez@correo.test", "Estrategia", 95m, 2);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.CreateAsync(request));
        Assert.Equal("A consultant with the same email already exists.", ex.Message);
    }

    [Fact]
    public async Task CreateAsync_TarifaFueraDeRango_LanzaValidacion()
    {
        var service = CreateService(new FakeConsultorRepository());
        var request = new ConsultorCreateRequest(
            "Ana Martinez Ponce", "ana.martinez@correo.test", "Estrategia", 250m, 2);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => service.CreateAsync(request));
        Assert.Single(ex.Errors);
    }

    [Fact]
    public async Task CreateAsync_DatoValido_InsertaYDevuelveDto()
    {
        var service = CreateService(new FakeConsultorRepository());
        var request = new ConsultorCreateRequest(
            "Ana Martinez Ponce", "ana.martinez@correo.test", "Estrategia", 95m, 2);

        var dto = await service.CreateAsync(request);

        Assert.True(dto.Id > 0);
        Assert.Equal("Ana Martinez Ponce", dto.NombreCompleto);
        Assert.True(dto.Activo);
        Assert.Equal(2, dto.ProyectosActivos);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var service = CreateService(new FakeConsultorRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task UpdateAsync_Inexistente_LanzaNotFound()
    {
        var service = CreateService(new FakeConsultorRepository());
        var request = new ConsultorUpdateRequest("A", "a@correo.test", "Area", 50m, true, 0);
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateAsync(999, request));
    }

    [Fact]
    public async Task DeleteAsync_Inexistente_LanzaNotFound()
    {
        var service = CreateService(new FakeConsultorRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => service.DeleteAsync(999));
    }

    [Fact]
    public async Task DeleteAsync_Existente_MarcaInactivo()
    {
        var repository = new FakeConsultorRepository();
        var id = await repository.InsertAsync(new Consultor
        {
            NombreCompleto = "Ana",
            Email = "ana@correo.test",
            Area = "Area",
            TarifaHora = 90m,
            Activo = true
        });

        var service = CreateService(repository);
        await service.DeleteAsync(id);

        var after = await repository.GetByIdAsync(id);
        Assert.False(after!.Activo);
    }
}