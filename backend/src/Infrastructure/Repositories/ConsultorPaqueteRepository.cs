using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class ConsultorPaqueteRepository : IConsultorPaqueteRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ConsultorPaqueteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsAsync(int consultorId, int paqueteId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand(
            """
            SELECT COUNT(1)
            FROM dbo.ConsultorPaquete
            WHERE ConsultorId = @ConsultorId AND PaqueteId = @PaqueteId
            """, connection);

        command.AddParameter("@ConsultorId", consultorId);
        command.AddParameter("@PaqueteId", paqueteId);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result) > 0;
    }

    public async Task<int> CountByConsultorAsync(int consultorId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand(
            """
            SELECT COUNT(1)
            FROM dbo.ConsultorPaquete
            WHERE ConsultorId = @ConsultorId
            """, connection);

        command.AddParameter("@ConsultorId", consultorId);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task<bool> AssignAsync(int consultorId, int paqueteId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_ConsultorPaquete_Assign", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@ConsultorId", consultorId);
        command.AddParameter("@PaqueteId", paqueteId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    public async Task<bool> UnassignAsync(int consultorId, int paqueteId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_ConsultorPaquete_Unassign", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@ConsultorId", consultorId);
        command.AddParameter("@PaqueteId", paqueteId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    public async Task<IReadOnlyList<ConsultorPaqueteItem>> ListByConsultorAsync(int consultorId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_ConsultorPaquete_ListByConsultor", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@ConsultorId", consultorId);

        var items = new List<ConsultorPaqueteItem>();

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(new ConsultorPaqueteItem(
                PaqueteId: reader.GetInt32(0),
                Nombre: reader.GetString(1),
                Descripcion: reader.IsDBNull(2) ? null : reader.GetString(2),
                Area: reader.GetString(3),
                Precio: reader.GetDecimal(4),
                Activo: reader.GetBoolean(5),
                FechaAsignacion: reader.GetDateTime(6)));
        }

        return items;
    }
}