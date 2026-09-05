using Consultora.Application.Common;

namespace Consultora.Application.Queries;

/// <summary>
/// Filtros + paginacion para el reporte de consultores top facturacion.
/// </summary>
public record ConsultoresTopFacturacionQuery(
    PageRequest Page,
    string? Area = null,
    bool? Activo = null);