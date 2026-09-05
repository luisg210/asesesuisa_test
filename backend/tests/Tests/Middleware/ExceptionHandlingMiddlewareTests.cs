using System.Net;
using System.Text;
using Consultora.Api.Middleware;
using Consultora.Application.Common;
using Consultora.Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace Consultora.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int Status, string Body)> ExecuteAsync(Exception exception)
    {
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        return (context.Response.StatusCode, body);
    }

    [Fact]
    public async Task NotFoundException_Devuelve404()
    {
        var (status, body) = await ExecuteAsync(new NotFoundException("Paquete 5 does not exist."));
        Assert.Equal((int)HttpStatusCode.NotFound, status);
        Assert.Contains("Paquete 5 does not exist.", body);
    }

    [Fact]
    public async Task ConflictException_Devuelve409()
    {
        var (status, body) = await ExecuteAsync(new ConflictException("Already exists."));
        Assert.Equal((int)HttpStatusCode.Conflict, status);
        Assert.Contains("false", body);
    }

    [Fact]
    public async Task UnauthorizedException_Devuelve401()
    {
        var (status, body) = await ExecuteAsync(new UnauthorizedException("Invalid email or password."));
        Assert.Equal((int)HttpStatusCode.Unauthorized, status);
        Assert.Contains("Invalid email or password.", body);
    }

    [Fact]
    public async Task ValidationException_Devuelve400ConErrores()
    {
        var validation = new ValidationException(new[]
        {
            new ValidationFailure("Email", "Email must be a valid email address.")
        });

        var (status, body) = await ExecuteAsync(validation);

        Assert.Equal((int)HttpStatusCode.BadRequest, status);
        Assert.Contains("Email must be a valid email address.", body);
        Assert.Contains("\"propertyName\"", body);
    }

    [Fact]
    public async Task ExceptionGenerica_Devuelve500()
    {
        var (status, body) = await ExecuteAsync(new InvalidOperationException("boom"));
        Assert.Equal((int)HttpStatusCode.InternalServerError, status);
        Assert.Contains("An unexpected error occurred.", body);
    }
}