namespace Consultora.Application.Dtos;

/// <summary>
/// Fila del reporte de consultores top por facturacion estimada.
/// </summary>
public record ConsultorFacturacionDto(
    int Id,
    string NombreCompleto,
    string Email,
    string Area,
    decimal TarifaHora,
    int ProyectosActivos,
    decimal FacturacionEstimada);