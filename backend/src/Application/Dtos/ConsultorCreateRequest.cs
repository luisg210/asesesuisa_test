namespace Consultora.Application.Dtos;

/// <summary>
/// Payload de creacion de consultor.
/// </summary>
public record ConsultorCreateRequest(
    string NombreCompleto,
    string Email,
    string Area,
    decimal TarifaHora,
    int ProyectosActivos = 0);