using Consultora.Application.Exceptions;
using Consultora.Application.Services;
using Consultora.Domain.Entities;
using Consultora.Tests.Fakes;

namespace Consultora.Tests.Services;

public class ConsultorPaqueteServiceTests
{
    private static ConsultorPaqueteService CreateService(
        FakeConsultorRepository consultores,
        FakePaqueteRepository paquetes,
        FakeConsultorPaqueteRepository asignaciones,
        FakeAuditService? audit = null)
        => new(asignaciones, consultores, paquetes, audit ?? new FakeAuditService());

    private static (FakeConsultorRepository Consultores, FakePaqueteRepository Paquetes) SeedData()
    {
        var consultores = new FakeConsultorRepository();
        var paquetes = new FakePaqueteRepository();

        consultores.InsertAsync(new Consultor
        {
            NombreCompleto = "Ana Martinez",
            Email = "ana@correo.test",
            Area = "Estrategia",
            TarifaHora = 95m,
            Activo = true
        }).GetAwaiter().GetResult();

        consultores.InsertAsync(new Consultor
        {
            NombreCompleto = "Consultor Inactivo",
            Email = "inactivo@correo.test",
            Area = "Finanzas",
            TarifaHora = 80m,
            Activo = false
        }).GetAwaiter().GetResult();

        paquetes.InsertAsync(new Paquete
        {
            Nombre = "Plan Digital",
            Area = "Tecnologia",
            Precio = 5000m,
            Activo = true
        }).GetAwaiter().GetResult();

        paquetes.InsertAsync(new Paquete
        {
            Nombre = "Paquete Inactivo",
            Area = "Comercial",
            Precio = 1000m,
            Activo = false
        }).GetAwaiter().GetResult();

        return (consultores, paquetes);
    }

    [Fact]
    public async Task Assign_ConsultorInexistente_LanzaNotFound()
    {
        var (consultores, paquetes) = SeedData();
        var service = CreateService(consultores, paquetes, new FakeConsultorPaqueteRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.AssignAsync(999, 1));
    }

    [Fact]
    public async Task Assign_PaqueteInactivo_LanzaConflict()
    {
        var (consultores, paquetes) = SeedData();
        var service = CreateService(consultores, paquetes, new FakeConsultorPaqueteRepository());

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.AssignAsync(1, 2));
        Assert.Equal("Cannot assign an inactive paquete to a consultant.", ex.Message);
    }

    [Fact]
    public async Task Assign_ConsultorInactivo_LanzaConflict()
    {
        var (consultores, paquetes) = SeedData();
        var service = CreateService(consultores, paquetes, new FakeConsultorPaqueteRepository());

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.AssignAsync(2, 1));
        Assert.Equal("Cannot assign paquetes to an inactive consultant.", ex.Message);
    }

    [Fact]
    public async Task Assign_Duplicada_LanzaConflict()
    {
        var (consultores, paquetes) = SeedData();
        var asignaciones = new FakeConsultorPaqueteRepository();
        asignaciones.Seed(1, 1);
        var service = CreateService(consultores, paquetes, asignaciones);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.AssignAsync(1, 1));
        Assert.Equal("The paquete is already assigned to this consultant.", ex.Message);
    }

    [Fact]
    public async Task Assign_ConCincoPaquetes_RechazaSextoConConflict()
    {
        var (consultores, paquetes) = SeedData();
        for (var id = 3; id <= 8; id++)
        {
            await paquetes.InsertAsync(new Paquete { Nombre = $"Paquete {id}", Area = "General", Precio = 1000m, Activo = true });
        }

        var asignaciones = new FakeConsultorPaqueteRepository();
        var service = CreateService(consultores, paquetes, asignaciones);

        foreach (var id in new[] { 1, 3, 4, 5, 6 })
        {
            await service.AssignAsync(1, id);
        }

        var ex = await Assert.ThrowsAsync<ConflictException>(() => service.AssignAsync(1, 7));
        Assert.Equal("A consultant can have at most 5 paquetes assigned.", ex.Message);
        Assert.Equal(5, (await service.ListByConsultorAsync(1)).Count);
    }

    [Fact]
    public async Task Assign_HastaCincoPaquetes_PermiteQuinto()
    {
        var (consultores, paquetes) = SeedData();
        for (var id = 3; id <= 6; id++)
        {
            await paquetes.InsertAsync(new Paquete { Nombre = $"Paquete {id}", Area = "General", Precio = 1000m, Activo = true });
        }

        var asignaciones = new FakeConsultorPaqueteRepository();
        var service = CreateService(consultores, paquetes, asignaciones);

        foreach (var id in new[] { 1, 3, 4, 5, 6 })
        {
            await service.AssignAsync(1, id);
        }

        Assert.Equal(5, (await service.ListByConsultorAsync(1)).Count);
    }

    [Fact]
    public async Task Assign_Valida_AsignaYAudita()
    {
        var (consultores, paquetes) = SeedData();
        var audit = new FakeAuditService();
        var service = CreateService(consultores, paquetes, new FakeConsultorPaqueteRepository(), audit);

        await service.AssignAsync(1, 1, actor: "admin@consultora.test");

        var assigned = await service.ListByConsultorAsync(1);
        Assert.Single(assigned);
        Assert.Equal(1, assigned[0].PaqueteId);

        var entry = Assert.Single(audit.Entries);
        Assert.Equal("ASSIGN", entry.Action);
        Assert.Equal("admin@consultora.test", entry.Actor);
    }

    [Fact]
    public async Task Unassign_NoAsignado_LanzaNotFound()
    {
        var (consultores, paquetes) = SeedData();
        var service = CreateService(consultores, paquetes, new FakeConsultorPaqueteRepository());

        await Assert.ThrowsAsync<NotFoundException>(() => service.UnassignAsync(1, 1));
    }

    [Fact]
    public async Task Unassign_Asignado_EliminaYAudita()
    {
        var (consultores, paquetes) = SeedData();
        var asignaciones = new FakeConsultorPaqueteRepository();
        asignaciones.Seed(1, 1);
        var audit = new FakeAuditService();
        var service = CreateService(consultores, paquetes, asignaciones, audit);

        await service.UnassignAsync(1, 1, actor: "admin@consultora.test");

        Assert.Empty(await service.ListByConsultorAsync(1));
        Assert.Equal("UNASSIGN", Assert.Single(audit.Entries).Action);
    }
}