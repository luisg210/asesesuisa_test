namespace Consultora.Application.Dtos;

/// <summary>
/// Solicitud de login.
/// </summary>
public record LoginRequest(string Email, string Password);