namespace Consultora.Application.Dtos;

/// <summary>
/// Solicitud de logout: revoca todos los refresh tokens del usuario.
/// </summary>
public record LogoutRequest(string RefreshToken);