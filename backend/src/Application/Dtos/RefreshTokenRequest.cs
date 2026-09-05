namespace Consultora.Application.Dtos;

/// <summary>
/// Solicitud de renovacion usando el refresh token.
/// </summary>
public record RefreshTokenRequest(string RefreshToken);