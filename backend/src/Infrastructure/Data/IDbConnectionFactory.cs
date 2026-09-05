namespace Consultora.Infrastructure.Data;

public interface IDbConnectionFactory
{
    Task<Microsoft.Data.SqlClient.SqlConnection> CreateAsync(CancellationToken ct = default);
}
