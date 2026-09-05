namespace Consultora.Application.Security;

/// <summary>
/// Genera valores opacos de refresh token (aleatoriedad criptografica,
/// codificacion base64url).
/// </summary>
public interface IRefreshTokenGenerator
{
    string Generate();
}