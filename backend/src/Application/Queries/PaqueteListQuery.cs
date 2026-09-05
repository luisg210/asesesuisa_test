using Consultora.Application.Common;

namespace Consultora.Application.Queries;

/// <summary>
/// Filtros + paginacion para el listado de paquetes.
/// </summary>
public record PaqueteListQuery(
    PageRequest Page,
    string? Nombre = null,
    string? Area = null,
    bool? Activo = null);