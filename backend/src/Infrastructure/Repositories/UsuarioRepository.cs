using Consultora.Application.Ports;
using Consultora.Domain.Entities;
using Consultora.Domain.Enums;
using Consultora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Consultora.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UsuarioRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Login", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new Usuario
        {
            Id = reader.GetInt32(0),
            Email = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Rol = Enum.Parse<Rol>(reader.GetString(3))
        };
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var command = new SqlCommand("dbo.sp_Usuario_GetById", connection)
        {
            CommandType = System.Data.CommandType.StoredProcedure
        };
        command.AddParameter("@Id", id);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
        {
            return null;
        }

        return new Usuario
        {
            Id = reader.GetInt32(0),
            Email = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Rol = Enum.Parse<Rol>(reader.GetString(3))
        };
    }
}