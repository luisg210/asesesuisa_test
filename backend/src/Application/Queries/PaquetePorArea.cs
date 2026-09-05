namespace Consultora.Application.Queries;

/// <summary>
/// Resultado agregado del reporte de paquetes por area.
/// </summary>
public record PaquetePorArea(
    string Area,
    int TotalPaquetes,
    decimal TotalMonto,
    decimal PrecioMinimo,
    decimal PrecioMaximo);