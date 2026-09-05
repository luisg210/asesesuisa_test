using System.Security.Cryptography;

namespace Consultora.Application.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    /// <summary>64 bytes aleatorios -> token opaco de uso unico.</summary>
    public string Generate()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
}