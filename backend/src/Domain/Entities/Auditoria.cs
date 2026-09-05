namespace Consultora.Domain.Entities;

/// <summary>
/// Entidad de auditoria: registro de escrituras (quien, que, cuando y desde donde).
/// </summary>
public class Auditoria
{
    public int Id { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string Entidad { get; set; } = string.Empty;
    public int? EntidadId { get; set; }
    public string? Detalle { get; set; }
    public string? Ip { get; set; }
    public DateTime FechaHora { get; set; }
}