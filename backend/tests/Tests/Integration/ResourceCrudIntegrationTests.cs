using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Consultora.Tests.Integration;

[Collection("SqlServer")]
public class ResourceCrudIntegrationTests
{
    private readonly SqlServerFixture _fixture;

    public ResourceCrudIntegrationTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    private static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}";

    [SkippableFact]
    public async Task Paquete_FlujoCompleto_Lista_Crea_Lee_Actualiza_Desactiva()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        // Crear
        var nombre = Unique("Paquete IT");
        var create = await client.PostAsJsonAsync("/api/v1/paquetes", new
        {
            nombre,
            descripcion = "Creado en integration test",
            area = "Tecnologia",
            precio = 2500.00m
        });
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        var createdJson = JsonDocument.Parse(await create.Content.ReadAsStringAsync());
        var id = createdJson.RootElement.GetProperty("data").GetProperty("id").GetInt32();

        // Obtener por id
        var get = await client.GetAsync($"/api/v1/paquetes/{id}");
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        using (var doc = JsonDocument.Parse(await get.Content.ReadAsStringAsync()))
        {
            Assert.Equal(nombre, doc.RootElement.GetProperty("data").GetProperty("nombre").GetString());
            Assert.True(doc.RootElement.GetProperty("data").GetProperty("activo").GetBoolean());
        }

        // Listar
        var list = await client.GetAsync("/api/v1/paquetes?page=1&pageSize=10");
        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        using (var doc = JsonDocument.Parse(await list.Content.ReadAsStringAsync()))
        {
            Assert.True(doc.RootElement.GetProperty("data").GetProperty("totalCount").GetInt32() >= 1);
        }

        // Actualizar
        var update = await client.PutAsJsonAsync($"/api/v1/paquetes/{id}", new
        {
            nombre,
            descripcion = "Actualizado",
            area = "Tecnologia",
            precio = 2700.00m,
            activo = true
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);

        // Eliminacion logica
        var delete = await client.DeleteAsync($"/api/v1/paquetes/{id}");
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        var getAfter = await client.GetAsync($"/api/v1/paquetes/{id}");
        using (var doc = JsonDocument.Parse(await getAfter.Content.ReadAsStringAsync()))
        {
            Assert.False(doc.RootElement.GetProperty("data").GetProperty("activo").GetBoolean());
        }
    }

    [SkippableFact]
    public async Task Paquete_PrecioNegativo_Devuelve400()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/paquetes", new
        {
            nombre = Unique("Invalido"),
            area = "Finanzas",
            precio = -10
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [SkippableFact]
    public async Task Paquete_Inexistente_Devuelve404()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var response = await client.GetAsync("/api/v1/paquetes/999999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [SkippableFact]
    public async Task Consultor_EmailDuplicado_Devuelve409()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var first = await client.PostAsJsonAsync("/api/v1/consultores", new
        {
            nombreCompleto = Unique("Persona"),
            email = $"dup-{Guid.NewGuid():N}@correo.test",
            area = "Finanzas",
            tarifaHora = 90.00m,
            proyectosActivos = 1
        });
        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        var json = JsonDocument.Parse(await first.Content.ReadAsStringAsync());
        var email = json.RootElement.GetProperty("data").GetProperty("email").GetString();

        var duplicate = await client.PostAsJsonAsync("/api/v1/consultores", new
        {
            nombreCompleto = Unique("Otra Persona"),
            email,
            area = "Finanzas",
            tarifaHora = 95.00m,
            proyectosActivos = 1
        });

        Assert.Equal(HttpStatusCode.Conflict, duplicate.StatusCode);
    }

    [SkippableFact]
    public async Task User_IntentaCrearPaquete_Devuelve403()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddUserTokenAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/paquetes", new
        {
            nombre = Unique("No permitido"),
            area = "Finanzas",
            precio = 100
        });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [SkippableFact]
    public async Task Reporte_PaquetesPorArea_DevuelveDatos()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var response = await client.GetAsync("/api/v1/reportes/paquetes-por-area?sortBy=TotalMonto&sortDir=desc");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        Assert.Contains("totalCount", data.EnumerateObject().Select(p => p.Name));
        Assert.True(data.GetProperty("totalCount").GetInt32() >= 1);
    }

    [SkippableFact]
    public async Task Areas_DevuelveCatalogo()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var response = await client.GetAsync("/api/v1/areas");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        Assert.True(data.GetArrayLength() >= 1);
    }

    [SkippableFact]
    public async Task Auditoria_AccesibleSoloParaAdmin()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var adminClient = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(adminClient);

        var adminResponse = await adminClient.GetAsync("/api/v1/auditoria?page=1&pageSize=5");
        Assert.Equal(HttpStatusCode.OK, adminResponse.StatusCode);

        using var userClient = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddUserTokenAsync(userClient);
        var userResponse = await userClient.GetAsync("/api/v1/auditoria");
        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }
}