using Consultora.Domain.Entities;

namespace Consultora.Application.Ports;

public interface IRefreshTokenRepository
{
    Task<int> InsertAsync(RefreshToken token, CancellationToken ct = default);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default);
    Task<bool> RevokeAsync(int id, CancellationToken ct = default);
    Task<int> RevokeAllByUserAsync(int usuarioId, CancellationToken ct = default);
}