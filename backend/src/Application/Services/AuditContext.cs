namespace Consultora.Application.Services;

/// <summary>
/// Datos minimos de una entrada de auditoria. El actor (usuario) lo provee el
/// controlador a partir de los claims del JWT.
/// </summary>
public record AuditContext(
    string? Actor,
    string Action,
    string Entity,
    int? EntityId = null,
    string? Detail = null,
    string? Ip = null);