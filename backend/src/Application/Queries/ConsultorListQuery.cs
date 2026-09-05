using Consultora.Application.Common;

namespace Consultora.Application.Queries;

/// <summary>
/// Filtros + paginacion para el listado de consultores.
/// </summary>
public record ConsultorListQuery(
    PageRequest Page,
    string? Nombre = null,
    string? Area = null,
    bool? Activo = null);