namespace Consultora.Domain.Entities;

/// <summary>
/// Relacion N:N entre consultores y paquetes (asignacion de consultores a paquetes).
/// </summary>
public class ConsultorPaquete
{
    public int ConsultorId { get; set; }
    public int PaqueteId { get; set; }
    public DateTime FechaAsignacion { get; set; }
}