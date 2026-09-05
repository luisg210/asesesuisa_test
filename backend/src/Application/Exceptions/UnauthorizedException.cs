namespace Consultora.Application.Exceptions;

/// <summary>
/// Credenciales invalidas o no autenticado -> HTTP 401.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}