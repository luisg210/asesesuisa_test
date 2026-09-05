using Consultora.Application.Dtos;

namespace Consultora.Application.Services.Contracts;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default, string? ip = null);
    Task<LoginResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken ct = default, string? ip = null);
    Task LogoutAsync(string refreshToken, CancellationToken ct = default, string? ip = null);
}