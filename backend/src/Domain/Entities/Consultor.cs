namespace Consultora.Domain.Entities;

/// <summary>
/// Entidad Consultor: profesional asignable a proyectos de la firma.
/// </summary>
public class Consultor
{
    public int Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public decimal TarifaHora { get; set; }
    public bool Activo { get; set; }
    public int ProyectosActivos { get; set; }
    public DateTime FechaCreacion { get; set; }
}