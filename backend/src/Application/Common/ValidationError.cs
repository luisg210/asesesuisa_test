namespace Consultora.Application.Common;

/// <summary>
/// Error de validacion individual (property + message).
/// </summary>
public record ValidationError(string PropertyName, string ErrorMessage);