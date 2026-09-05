using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Inicia sesion y devuelve JWT + refresh token.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(LoginRequest request, CancellationToken ct)
    {
        var response = await _authService.LoginAsync(request, ct, ClientIp);
        return Ok(ApiResponse<LoginResponse>.Ok(response, "Login successful."));
    }

    /// <summary>
    /// Renueva el JWT usando el refresh token (rota el refresh token).
    /// </summary>
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Refresh(RefreshTokenRequest request, CancellationToken ct)
    {
        var response = await _authService.RefreshAsync(request, ct, ClientIp);
        return Ok(ApiResponse<LoginResponse>.Ok(response, "Token refreshed."));
    }

    /// <summary>
    /// Revoca todos los refresh tokens del usuario (sesion cerrada).
    /// </summary>
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout(LogoutRequest request, CancellationToken ct)
    {
        await _authService.LogoutAsync(request.RefreshToken, ct, ClientIp);
        return Ok(ApiResponse<object>.Ok(null!, "Logout successful."));
    }
}