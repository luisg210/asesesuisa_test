using Consultora.Application.Common;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class PaqueteRepository : IPaqueteRepository
{
    private static readonly string[] SortableColumns =
        { "Id", "Nombre", "Area", "Precio", "FechaCreacion" };

    private readonly IDbConnectionFactory _connectionFactory;

    public PaqueteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<Paquete>> ListAsync(PaqueteListQuery query, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Paquetes_List", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.AddParameter("@Page", query.Page.SafePage);
        command.AddParameter("@PageSize", query.Page.SafePageSize);
        command.AddParameter("@SortBy", NormalizeSort(query.Page.SafeSortBy));
        command.AddParameter("@SortDir", query.Page.SafeSortDir);
        command.AddParameter("@Nombre", query.Nombre, nullable: true);
        command.AddParameter("@Area", query.Area, nullable: true);
        command.AddParameter("@Activo", query.Activo, nullable: true);

        var items = new List<Paquete>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(Map(reader));
            totalCount = reader.GetInt32(reader.FieldCount - 1);
        }

        return PagedResult<Paquete>.Create(items, totalCount, query.Page.SafePage, query.Page.SafePageSize);
    }

    public async Task<Paquete?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Paquetes_GetById", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Map(reader) : null;
    }

    public async Task<int> InsertAsync(Paquete paquete, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Paquetes_Insert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Nombre", paquete.Nombre);
        command.AddParameter("@Descripcion", paquete.Descripcion, nullable: true);
        command.AddParameter("@Area", paquete.Area);
        command.AddParameter("@Precio", paquete.Precio);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Paquete paquete, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Paquetes_Update", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", paquete.Id);
        command.AddParameter("@Nombre", paquete.Nombre);
        command.AddParameter("@Descripcion", paquete.Descripcion, nullable: true);
        command.AddParameter("@Area", paquete.Area);
        command.AddParameter("@Precio", paquete.Precio);
        command.AddParameter("@Activo", paquete.Activo);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Paquetes_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    private static string NormalizeSort(string sortBy)
        => SortableColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase) ? sortBy : "Id";

    private static Paquete Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Nombre = reader.GetString(1),
        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
        Area = reader.GetString(3),
        Precio = reader.GetDecimal(4),
        Activo = reader.GetBoolean(5),
        FechaCreacion = reader.GetDateTime(6)
    };
}