namespace Consultora.Application.Dtos;

/// <summary>
/// Paquete de servicio (salida).
/// </summary>
public record PaqueteDto(
    int Id,
    string Nombre,
    string? Descripcion,
    string Area,
    decimal Precio,
    bool Activo,
    DateTime FechaCreacion);