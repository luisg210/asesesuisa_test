namespace Consultora.Application.Dtos;

/// <summary>
/// Paquete asignado a un consultor (salida).
/// </summary>
public record ConsultorPaqueteDto(
    int PaqueteId,
    string Nombre,
    string? Descripcion,
    string Area,
    decimal Precio,
    bool Activo,
    DateTime FechaAsignacion);