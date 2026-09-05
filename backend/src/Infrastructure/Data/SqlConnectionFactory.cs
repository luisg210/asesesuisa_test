namespace Consultora.Infrastructure.Data;

/// <summary>
/// Fabrica de conexiones ADO.NET (SqlConnection ya abierta).
/// </summary>
public class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly DatabaseOptions _options;

    public SqlConnectionFactory(Microsoft.Extensions.Options.IOptions<DatabaseOptions> options)
    {
        _options = options.Value;
    }

    public async Task<Microsoft.Data.SqlClient.SqlConnection> CreateAsync(CancellationToken ct = default)
    {
        var connection = new Microsoft.Data.SqlClient.SqlConnection(_options.ConsultoraDb);
        await connection.OpenAsync(ct);
        return connection;
    }
}
