namespace Consultora.Application.Dtos;

/// <summary>
/// Payload de actualizacion de consultor.
/// </summary>
public record ConsultorUpdateRequest(
    string NombreCompleto,
    string Email,
    string Area,
    decimal TarifaHora,
    bool Activo,
    int ProyectosActivos);