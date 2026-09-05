using Consultora.Application.Common;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private static readonly string[] SortableColumns = { "FechaHora", "Usuario", "Entidad", "Accion" };

    private readonly IDbConnectionFactory _connectionFactory;

    public AuditoriaRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertAsync(Auditoria auditoria, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Auditoria_Insert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Usuario", auditoria.Usuario);
        command.AddParameter("@Accion", auditoria.Accion);
        command.AddParameter("@Entidad", auditoria.Entidad);
        command.AddParameter("@EntidadId", auditoria.EntidadId, nullable: true);
        command.AddParameter("@Detalle", auditoria.Detalle, nullable: true);
        command.AddParameter("@Ip", auditoria.Ip, nullable: true);

        await command.ExecuteNonQueryAsync(ct);
    }

    public async Task<PagedResult<Auditoria>> ListAsync(AuditoriaQuery query, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Auditoria_List", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.AddParameter("@Page", query.Page.SafePage);
        command.AddParameter("@PageSize", query.Page.SafePageSize);
        command.AddParameter("@SortBy", NormalizeSort(query.Page.SafeSortBy));
        command.AddParameter("@SortDir", query.Page.SafeSortDir);
        command.AddParameter("@Entidad", query.Entidad, nullable: true);
        command.AddParameter("@Accion", query.Accion, nullable: true);
        command.AddParameter("@Usuario", query.Usuario, nullable: true);

        var items = new List<Auditoria>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(Map(reader));
            totalCount = reader.GetInt32(reader.FieldCount - 1);
        }

        return PagedResult<Auditoria>.Create(items, totalCount, query.Page.SafePage, query.Page.SafePageSize);
    }

    private static string NormalizeSort(string sortBy)
        => SortableColumns.Contains(sortBy, StringComparer.OrdinalIgnoreCase) ? sortBy : "FechaHora";

    private static Auditoria Map(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Usuario = reader.GetString(1),
        Accion = reader.GetString(2),
        Entidad = reader.GetString(3),
        EntidadId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
        Detalle = reader.IsDBNull(5) ? null : reader.GetString(5),
        Ip = reader.IsDBNull(6) ? null : reader.GetString(6),
        FechaHora = reader.GetDateTime(7)
    };
}