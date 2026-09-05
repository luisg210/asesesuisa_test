using Consultora.Application.Common;

namespace Consultora.Application.Queries;

/// <summary>
/// Filtros + paginacion para la bitacora de auditoria.
/// </summary>
public record AuditoriaQuery(
    PageRequest Page,
    string? Entidad = null,
    string? Accion = null,
    string? Usuario = null);