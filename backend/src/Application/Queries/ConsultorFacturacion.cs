namespace Consultora.Application.Queries;

/// <summary>
/// Resultado agregado del reporte de top facturacion.
/// </summary>
public record ConsultorFacturacion(
    int Id,
    string NombreCompleto,
    string Email,
    string Area,
    decimal TarifaHora,
    int ProyectosActivos,
    decimal FacturacionEstimada);