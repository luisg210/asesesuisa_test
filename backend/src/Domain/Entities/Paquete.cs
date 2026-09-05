namespace Consultora.Domain.Entities;

/// <summary>
/// Entidad Paquete: paquete de servicio ofrecido por la firma.
/// </summary>
public class Paquete
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Area { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}