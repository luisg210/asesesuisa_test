namespace Consultora.Application.Dtos;

/// <summary>
/// Fila de la bitacora de auditoria (salida).
/// </summary>
public record AuditoriaDto(
    int Id,
    string Usuario,
    string Accion,
    string Entidad,
    int? EntidadId,
    string? Detalle,
    string? Ip,
    DateTime FechaHora);