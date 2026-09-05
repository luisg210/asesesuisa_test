namespace Consultora.Application.Dtos;

/// <summary>
/// Payload de creacion de paquete.
/// </summary>
public record PaqueteCreateRequest(string Nombre, string? Descripcion, string Area, decimal Precio);