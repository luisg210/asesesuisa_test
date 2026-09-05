namespace Consultora.Application.Dtos;

/// <summary>
/// Payload de actualizacion de paquete.
/// </summary>
public record PaqueteUpdateRequest(string Nombre, string? Descripcion, string Area, decimal Precio, bool Activo);