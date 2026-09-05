using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Consultora.Tests.Integration;

[Collection("SqlServer")]
public class AuthFlowIntegrationTests
{
    private readonly SqlServerFixture _fixture;

    public AuthFlowIntegrationTests(SqlServerFixture fixture)
    {
        _fixture = fixture;
    }

    [SkippableFact]
    public async Task Login_CredencialesValidas_DevuelveJwtYRefreshToken()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@consultora.test",
            password = "Admin@123"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("token").GetString()));
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("refreshToken").GetString()));
        Assert.Equal("Admin", data.GetProperty("role").GetString());
    }

    [SkippableFact]
    public async Task Login_PasswordIncorrecta_Devuelve401()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "admin@consultora.test",
            password = "Incorrecta456"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [SkippableFact]
    public async Task Refresh_TokenValido_RotaYDevuelveNuevaSesion()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        var (_, refreshToken) = await ApiTestHelpers.LoginAsync(client, "admin@consultora.test", "Admin@123");

        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        Assert.NotEqual(refreshToken, data.GetProperty("refreshToken").GetString());
        Assert.False(string.IsNullOrWhiteSpace(data.GetProperty("token").GetString()));
    }

    [SkippableFact]
    public async Task Refresh_TokenUsadoDosVeces_Devuelve401()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        var (_, refreshToken) = await ApiTestHelpers.LoginAsync(client, "admin@consultora.test", "Admin@123");

        var first = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        // La rotacion invalida el token anterior: reutilizarlo debe fallar.
        var second = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, second.StatusCode);
    }

    [SkippableFact]
    public async Task Refresh_TokenInexistente_Devuelve401()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken = "token-invalido" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [SkippableFact]
    public async Task Logout_RevocaElToken_YElRefreshPosteriorFalla()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        var (_, refreshToken) = await ApiTestHelpers.LoginAsync(client, "admin@consultora.test", "Admin@123");

        var logout = await client.PostAsJsonAsync("/api/v1/auth/logout", new { refreshToken });
        Assert.Equal(HttpStatusCode.OK, logout.StatusCode);

        var refresh = await client.PostAsJsonAsync("/api/v1/auth/refresh", new { refreshToken });
        Assert.Equal(HttpStatusCode.Unauthorized, refresh.StatusCode);
    }

    [SkippableFact]
    public async Task SinToken_Devuelve401()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();

        var response = await client.GetAsync("/api/v1/paquetes");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [SkippableFact]
    public async Task TokenValido_PermiteConsultarRecursos()
    {
        Skip.IfNot(_fixture.Available, "SQL Server no disponible (requiere Docker).");
        using var client = _fixture.ApiFactory.CreateClient();
        await ApiTestHelpers.AddAdminTokenAsync(client);

        var response = await client.GetAsync("/api/v1/paquetes");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}