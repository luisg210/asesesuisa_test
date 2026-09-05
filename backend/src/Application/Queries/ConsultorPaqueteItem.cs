namespace Consultora.Application.Queries;

/// <summary>
/// Paquete asignado a un consultor (resultado de consulta).
/// </summary>
public record ConsultorPaqueteItem(
    int PaqueteId,
    string Nombre,
    string? Descripcion,
    string Area,
    decimal Precio,
    bool Activo,
    DateTime FechaAsignacion);