namespace Consultora.Application.Exceptions;

/// <summary>
/// Recurso no encontrado -> HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}