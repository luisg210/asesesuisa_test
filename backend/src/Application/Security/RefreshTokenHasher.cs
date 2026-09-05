using System.Security.Cryptography;
using System.Text;

namespace Consultora.Application.Security;

/// <summary>
/// Calcula el hash SHA-256 (hex) de un refresh token. Solo el hash se persiste.
/// </summary>
public static class RefreshTokenHasher
{
    public static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)))
            .ToLowerInvariant();
}