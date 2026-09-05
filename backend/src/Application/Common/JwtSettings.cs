namespace Consultora.Application.Common;

/// <summary>
/// Configuracion JWT (section "Jwt" del appsettings).
/// </summary>
public class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>Identificador de la clave simetrica. Microsoft.IdentityModel exige
    /// "kid" en el token para validar la firma; se usa el mismo valor al firmar
    /// y al validar.</summary>
    public const string KeyId = "ConsultoraApi";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;

    /// <summary>Vida del refresh token (en minutos). Default: 7 dias.</summary>
    public int RefreshTokenExpiryMinutes { get; set; } = 10080;
}