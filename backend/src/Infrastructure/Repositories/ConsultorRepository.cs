using Consultora.Application.Common;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class ConsultorRepository : IConsultorRepository
{
    private static readonly string[] SortableColumns =
        { "Id", "NombreCompleto", "Email", "Area", "TarifaHora", "ProyectosActivos", "FechaCreacion" };

    private readonly IDbConnectionFactory _connectionFactory;

    public ConsultorRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<Consultor>> ListAsync(ConsultorListQuery query, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Consultores_List", connection)
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

        var items = new List<Consultor>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(Map(reader));
            totalCount = reader.GetInt32(reader.FieldCount - 1);
        }

        return PagedResult<Consultor>.Create(items, totalCount, query.Page.SafePage, query.Page.SafePageSize);
    }

    public async Task<Consultor?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Consultores_GetById", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) ? Map(reader) : null;
    }

    public async Task<int> InsertAsync(Consultor consultor, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Consultores_Insert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@NombreCompleto", consultor.NombreCompleto);
        command.AddParameter("@Email", consultor.Email);
        command.AddParameter("@Area", consultor.Area);
        command.AddParameter("@TarifaHora", consultor.TarifaHora);
        command.AddParameter("@ProyectosActivos", consultor.ProyectosActivos);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task<bool> UpdateAsync(Consultor consultor, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Consultores_Update", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", consultor.Id);
        command.AddParameter("@NombreCompleto", consultor.NombreCompleto);
        command.AddParameter("@Email", consultor.Email);
        command.AddParameter("@Area", consultor.Area);
        command.AddParameter("@TarifaHora", consultor.TarifaHora);
        command.AddParameter("@Activo", consultor.Activo);
        command.AddParameter("@ProyectosActivos", consultor.ProyectosActivos);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    public async Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Consultores_Delete", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(ct);
        return await reader.ReadAsync(ct) && reader.GetInt32(0) > 0;
    }

    public async Task<bool> ExistsByNameAndAreaAsync(string nombreCompleto, string area, int excludeId = 0, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand(
            """
            SELECT COUNT(1)
            FROM dbo.Consultores
            WHERE NombreCompleto = @NombreCompleto AND Area = @Area AND Id <> @ExcludeId
            """, connection);

        command.AddParameter("@NombreCompleto", nombreCompleto);
        command.AddParameter("@Area", area);
        command.AddParameter("@ExcludeId", excludeId);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result) > 0;
    }

    public async Task<bool> ExistsByEmailAsync(string email, int excludeId = 0, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand(
            """
            SELECT COUNT(1)
            FROM dbo.Consultores
            WHERE Email = @Email AND Id <> @ExcludeId
            """, connection);

        command.AddParameter("@Email", email);
        command.AddParameter("@ExcludeId", excludeId);

        var result = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result) > 0;
    }

    private static string NormalizeSort(string sortBy)
        => SortableColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase) ? sortBy : "Id";

    private static Consultor Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        NombreCompleto = reader.GetString(1),
        Email = reader.GetString(2),
        Area = reader.GetString(3),
        TarifaHora = reader.GetDecimal(4),
        Activo = reader.GetBoolean(5),
        ProyectosActivos = reader.GetInt32(6),
        FechaCreacion = reader.GetDateTime(7)
    };
}