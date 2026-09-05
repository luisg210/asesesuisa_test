using Consultora.Application.Ports;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

/// <summary>
/// Catalogo de areas derivado de los valores existentes en Paquetes y
/// Consultores (consulta directa, no requiere script de base de datos).
/// </summary>
public class AreaRepository : IAreaRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AreaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand(
            """
            SELECT Area FROM dbo.Paquetes
            UNION
            SELECT Area FROM dbo.Consultores
            ORDER BY Area
            """, connection);

        var areas = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            areas.Add(reader.GetString(0));
        }

        return areas;
    }
}