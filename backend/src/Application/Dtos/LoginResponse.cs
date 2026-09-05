namespace Consultora.Application.Dtos;

/// <summary>
/// Respuesta de login: JWT + refresh token opaco + datos basicos del usuario.
/// </summary>
public record LoginResponse(
    string Token,
    DateTime ExpiresAt,
    string RefreshToken,
    DateTime RefreshExpiresAt,
    string Email,
    string Role);