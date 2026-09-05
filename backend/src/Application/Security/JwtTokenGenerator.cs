using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Consultora.Application.Common;
using Consultora.Domain.Entities;
using Consultora.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace Consultora.Application.Security;

/// <summary>
/// Genera JWTs firmados con los claims de email y rol del usuario.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtSettings _settings;

    public JwtTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenResult Generate(Usuario user)
    {
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(_settings.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Rol.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey))
        {
            KeyId = JwtSettings.KeyId
        };
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
        return new TokenResult(tokenValue, expiresAt);
    }
}