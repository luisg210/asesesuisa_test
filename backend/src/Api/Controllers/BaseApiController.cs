using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

/// <summary>
/// Base de los controladores: expone el usuario autenticado y la IP del
/// cliente para operaciones de escritura y auditoria.
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    /// <summary>Email del usuario autenticado (claim del JWT).</summary>
    protected string CurrentUserEmail
        => User.FindFirstValue(ClaimTypes.Email) ?? "system";

    /// <summary>Direccion IP remota del cliente (o nula si no esta disponible).</summary>
    protected string? ClientIp
        => HttpContext.Connection.RemoteIpAddress?.ToString();
}