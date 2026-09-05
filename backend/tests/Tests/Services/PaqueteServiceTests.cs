using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Services;
using Consultora.Application.Validation;
using Consultora.Tests.Fakes;

namespace Consultora.Tests.Services;

public class PaqueteServiceTests
{
    private static PaqueteService CreateService(FakePaqueteRepository repository)
        => new(repository, new PaqueteCreateValidator(), new PaqueteUpdateValidator());

    [Fact]
    public async Task CreateAsync_PrecioNegativo_LanzaValidacion()
    {
        var service = CreateService(new FakePaqueteRepository());
        var request = new PaqueteCreateRequest("Paquete", "Desc", "Area", -1m);

        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => service.CreateAsync(request));
    }

    [Fact]
    public async Task CreateAsync_Valido_DevuelveDtoConId()
    {
        var service = CreateService(new FakePaqueteRepository());
        var request = new PaqueteCreateRequest("Diagnostico", "Descripcion", "Estrategia", 3500m);

        var dto = await service.CreateAsync(request);

        Assert.True(dto.Id > 0);
        Assert.Equal("Diagnostico", dto.Nombre);
        Assert.True(dto.Activo);
    }

    [Fact]
    public async Task GetByIdAsync_Inexistente_LanzaNotFound()
    {
        var service = CreateService(new FakePaqueteRepository());
        await Assert.ThrowsAsync<NotFoundException>(() => service.GetByIdAsync(999));
    }

    [Fact]
    public async Task DeleteAsync_Existente_MarcaInactivo()
    {
        var repository = new FakePaqueteRepository();
        var id = await repository.InsertAsync(new()
        {
            Nombre = "P",
            Area = "A",
            Precio = 100m,
            Activo = true
        });

        var service = CreateService(repository);
        await service.DeleteAsync(id);

        var after = await repository.GetByIdAsync(id);
        Assert.False(after!.Activo);
    }
}