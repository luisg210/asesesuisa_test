namespace Consultora.Application.Dtos;

/// <summary>
/// Consultor (salida).
/// </summary>
public record ConsultorDto(
    int Id,
    string NombreCompleto,
    string Email,
    string Area,
    decimal TarifaHora,
    bool Activo,
    int ProyectosActivos,
    DateTime FechaCreacion);