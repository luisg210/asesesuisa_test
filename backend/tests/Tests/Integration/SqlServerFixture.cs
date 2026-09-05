using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;

namespace Consultora.Tests.Integration;

/// <summary>
/// Fixture compartido: levanta un SQL Server real (Docker via Testcontainers),
/// inicializa la base con los scripts del repositorio y construye la API
/// (<c>WebApplicationFactory&lt;Program&gt;</c>) apuntando a esa base.
/// Si Docker no esta disponible, <c>Available</c> queda en false y los tests
/// se reportan como "skipped".
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    private const string SaPassword = "Consult0ra!Passw0rd";
    private const string JwtSecret = "integration-test-secret-key-1234567890-abcdef-0123456789";

    private readonly MsSqlContainer _container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .WithPassword(SaPassword)
        .Build();

    public bool Available { get; private set; }

    public string DatabaseConnectionString { get; private set; } = string.Empty;

    public WebApplicationFactory<Program> ApiFactory { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        try
        {
            await _container.StartAsync();

            var masterConnection = _container.GetConnectionString();
            DatabaseConnectionString = $"{masterConnection};Database=ConsultoraDb;";

            await SqlScriptRunner.RunScriptsAsync(
                DatabaseConnectionString,
                (ScriptPath: "Sql/01_Create_Database.sql", ConnectionString: masterConnection),
                (ScriptPath: "Sql/02_Procedimientos.sql", ConnectionString: DatabaseConnectionString),
                (ScriptPath: "Sql/03_Seed_Data.sql", ConnectionString: DatabaseConnectionString),
                (ScriptPath: "Sql/05_RefreshTokens.sql", ConnectionString: DatabaseConnectionString));

            ApiFactory = CreateApiFactory(DatabaseConnectionString);
            Available = true;
        }
        catch
        {
            Available = false;
        }
    }

    public async Task DisposeAsync()
    {
        if (ApiFactory is not null)
        {
            await ApiFactory.DisposeAsync();
        }

        await _container.DisposeAsync();
    }

    private static WebApplicationFactory<Program> CreateApiFactory(string connectionString)
        => new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("IntegrationTest");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:ConsultoraDb"] = connectionString,
                    ["Jwt:SecretKey"] = JwtSecret,
                    ["Jwt:Issuer"] = "ConsultoraApi",
                    ["Jwt:Audience"] = "ConsultoraFrontend",
                    ["Jwt:ExpiryMinutes"] = "60",
                    ["Jwt:RefreshTokenExpiryMinutes"] = "60"
                });
            });
        });
}