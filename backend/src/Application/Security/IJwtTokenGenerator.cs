using Consultora.Domain.Entities;

namespace Consultora.Application.Security;

public interface IJwtTokenGenerator
{
    TokenResult Generate(Usuario user);
}