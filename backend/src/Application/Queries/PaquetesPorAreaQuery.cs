using Consultora.Application.Common;

namespace Consultora.Application.Queries;

/// <summary>
/// Filtros + paginacion para el reporte de paquetes por area.
/// </summary>
public record PaquetesPorAreaQuery(
    PageRequest Page,
    string? Area = null,
    bool? Activo = null);