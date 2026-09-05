namespace Consultora.Application.Exceptions;

/// <summary>
/// Conflicto con datos existentes (unicidad) -> HTTP 409.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
    }
}