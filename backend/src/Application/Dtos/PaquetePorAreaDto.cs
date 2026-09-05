namespace Consultora.Application.Dtos;

/// <summary>
/// Fila del reporte de paquetes por area.
/// </summary>
public record PaquetePorAreaDto(string Area, int TotalPaquetes, decimal TotalMonto, decimal PrecioMinimo, decimal PrecioMaximo);