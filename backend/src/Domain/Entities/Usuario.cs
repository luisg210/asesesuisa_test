using Consultora.Domain.Enums;

namespace Consultora.Domain.Entities;

/// <summary>
/// Entidad Usuario: cuenta de acceso al sistema.
/// </summary>
public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Rol Rol { get; set; }
}