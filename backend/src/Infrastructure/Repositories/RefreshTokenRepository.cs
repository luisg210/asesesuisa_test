using Consultora.Application.Ports;
using Consultora.Domain.Entities;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public RefreshTokenRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<int> InsertAsync(RefreshToken token, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_RefreshTokens_Insert", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@UsuarioId", token.UsuarioId);
        command.AddParameter("@TokenHash", token.TokenHash);
        command.AddParameter("@ExpiresAt", token.ExpiresAt);
        command.AddParameter("@Ip", token.Ip, nullable: true);

        var id = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(id);
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_RefreshTokens_GetByHash", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@TokenHash", tokenHash);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new RefreshToken
        {
            Id = reader.GetInt32(0),
            UsuarioId = reader.GetInt32(1),
            TokenHash = reader.GetString(2),
            ExpiresAt = reader.GetDateTime(3),
            RevokedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            Ip = reader.IsDBNull(5) ? null : reader.GetString(5)
        };
    }

    public async Task<bool> RevokeAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_RefreshTokens_Revoke", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        var affected = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(affected) > 0;
    }

    public async Task<int> RevokeAllByUserAsync(int usuarioId, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_RefreshTokens_RevokeAllByUser", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@UsuarioId", usuarioId);

        var affected = await command.ExecuteScalarAsync(ct);
        return Convert.ToInt32(affected);
    }
}