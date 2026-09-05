using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Consultora.Application.Common;
using Consultora.Application.Security;
using Consultora.Domain.Entities;
using Consultora.Domain.Enums;
using Microsoft.Extensions.Options;

namespace Consultora.Tests.Security;

public class JwtTokenGeneratorTests
{
    [Fact]
    public void Generate_DevuelveTokenConClaimsDeRolYEmail()
    {
        var settings = Options.Create(new JwtSettings
        {
            Issuer = "Test",
            Audience = "Test",
            SecretKey = "TestSecretKey_At_Least_32_Characters_Long_1234567890",
            ExpiryMinutes = 30
        });

        var generator = new JwtTokenGenerator(settings);
        var user = new Usuario { Id = 7, Email = "user@consultora.test", Rol = Rol.User };

        var result = generator.Generate(user);

        Assert.NotNull(result.Token);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(result.Token);

        Assert.Contains("User", jwt.Claims.Select(c => c.Value));
        Assert.Contains("user@consultora.test", jwt.Claims.Select(c => c.Value));
    }
}