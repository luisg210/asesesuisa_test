namespace Consultora.Application.Common;

/// <summary>
/// Respuesta estandar de la API.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Message = message ?? string.Empty, Data = data };

    public static ApiResponse<T> Fail(string message, T? data = default)
        => new() { Success = false, Message = message, Data = data };
}