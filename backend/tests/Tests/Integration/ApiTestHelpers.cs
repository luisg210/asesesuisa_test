using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Consultora.Tests.Integration;

/// <summary>
/// Helpers para invocar la API real en los integration tests.
/// </summary>
internal static class ApiTestHelpers
{
    public static HttpClient CreateAdminClient(SqlServerFixture fixture)
    {
        var client = fixture.ApiFactory.CreateClient();
        return client;
    }

    public static async Task AddAdminTokenAsync(HttpClient client)
    {
        var session = await LoginAsync(client, "admin@consultora.test", "Admin@123");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session.Token);
    }

    public static async Task AddUserTokenAsync(HttpClient client)
    {
        var session = await LoginAsync(client, "user@consultora.test", "User@123");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", session.Token);
    }

    public static async Task<(string Token, string RefreshToken)> LoginAsync(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = json.RootElement.GetProperty("data");
        return (data.GetProperty("token").GetString()!, data.GetProperty("refreshToken").GetString()!);
    }
}