using System.Net;
using System.Text;
using System.Text.Json;
using Consultora.Application.Common;
using Consultora.Application.Exceptions;
using FluentValidation;
using Microsoft.Data.SqlClient;

namespace Consultora.Api.Middleware;

/// <summary>
/// Middleware global de manejo de excepciones: mapea excepciones de
/// aplicacion a respuestas HTTP con el formato estandar de la API.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case ValidationException validation:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await WriteAsync(context,
                    new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed.",
                        Data = validation.Errors.Select(e =>
                            new ValidationError(e.PropertyName, e.ErrorMessage)).ToList()
                    });
                return;

            case ConflictException conflict:
                await WriteErrorAsync(context, HttpStatusCode.Conflict, conflict.Message);
                return;

            case NotFoundException notFound:
                await WriteErrorAsync(context, HttpStatusCode.NotFound, notFound.Message);
                return;

            case UnauthorizedException unauthorized:
                await WriteErrorAsync(context, HttpStatusCode.Unauthorized, unauthorized.Message);
                return;

            case SqlException sql when sql.Number is 2601 or 2627:
                // Respaldo ante violaciones de indice unico (unicidad garantizada tambien en BD).
                await WriteErrorAsync(context, HttpStatusCode.Conflict, "A record with the same unique values already exists.");
                return;

            default:
                await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
                return;
        }
    }

    private static Task WriteErrorAsync(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        return WriteAsync(context, ApiResponse<object>.Fail(message));
    }

    private static Task WriteAsync(HttpContext context, object body)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions), Encoding.UTF8);
    }
}