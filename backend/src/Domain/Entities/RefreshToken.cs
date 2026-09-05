namespace Consultora.Domain.Entities;

/// <summary>
/// Refresh token opaco: solo se persiste el hash SHA-256 del valor en claro.
/// Rota en cada renovacion y se revoca al hacer logout.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? Ip { get; set; }
}