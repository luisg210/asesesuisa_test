using Consultora.Application.Common;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class ReporteRepository : IReporteRepository
{
    private static readonly string[] PaquetesPorAreaSortable = { "Area", "TotalPaquetes", "TotalMonto" };
    private static readonly string[] TopFacturacionSortable =
        { "NombreCompleto", "Area", "TarifaHora", "ProyectosActivos", "FacturacionEstimada" };

    private readonly IDbConnectionFactory _connectionFactory;

    public ReporteRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PaquetePorArea>> PaquetesPorAreaAsync(PaquetesPorAreaQuery query, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Reporte_PaquetesPorArea", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.AddParameter("@Page", query.Page.SafePage);
        command.AddParameter("@PageSize", query.Page.SafePageSize);
        command.AddParameter("@SortBy", NormalizeSort(query.Page.SafeSortBy, PaquetesPorAreaSortable, "Area"));
        command.AddParameter("@SortDir", query.Page.SafeSortDir);
        command.AddParameter("@Area", query.Area, nullable: true);
        command.AddParameter("@Activo", query.Activo, nullable: true);

        var items = new List<PaquetePorArea>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(new PaquetePorArea(
                Area: reader.GetString(0),
                TotalPaquetes: reader.GetInt32(1),
                TotalMonto: reader.IsDBNull(2) ? decimal.Zero : reader.GetDecimal(2),
                PrecioMinimo: reader.IsDBNull(3) ? decimal.Zero : reader.GetDecimal(3),
                PrecioMaximo: reader.IsDBNull(4) ? decimal.Zero : reader.GetDecimal(4)));
            totalCount = reader.GetInt32(reader.FieldCount - 1);
        }

        return PagedResult<PaquetePorArea>.Create(items, totalCount, query.Page.SafePage, query.Page.SafePageSize);
    }

    public async Task<PagedResult<ConsultorFacturacion>> ConsultoresTopFacturacionAsync(
        ConsultoresTopFacturacionQuery query, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Reporte_ConsultoresTopFacturacion", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };

        command.AddParameter("@Page", query.Page.SafePage);
        command.AddParameter("@PageSize", query.Page.SafePageSize);
        command.AddParameter("@SortBy", NormalizeSort(query.Page.SafeSortBy, TopFacturacionSortable, "FacturacionEstimada"));
        command.AddParameter("@SortDir", query.Page.SafeSortDir);
        command.AddParameter("@Area", query.Area, nullable: true);
        command.AddParameter("@Activo", query.Activo, nullable: true);

        var items = new List<ConsultorFacturacion>();
        var totalCount = 0;

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(new ConsultorFacturacion(
                Id: reader.GetInt32(0),
                NombreCompleto: reader.GetString(1),
                Email: reader.GetString(2),
                Area: reader.GetString(3),
                TarifaHora: reader.GetDecimal(4),
                ProyectosActivos: reader.GetInt32(5),
                FacturacionEstimada: reader.GetDecimal(6)));
            totalCount = reader.GetInt32(reader.FieldCount - 1);
        }

        return PagedResult<ConsultorFacturacion>.Create(items, totalCount, query.Page.SafePage, query.Page.SafePageSize);
    }

    private static string NormalizeSort(string sortBy, IEnumerable<string> allowed, string defaultValue)
        => allowed.Contains(sortBy, StringComparer.OrdinalIgnoreCase) ? sortBy : defaultValue;
}