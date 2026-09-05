using Consultora.Application.Common;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Application.Security;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Consultora.Domain.Entities;

namespace Consultora.Tests.Fakes;

/// <summary>
/// Repositorio en memoria para aislar tests de servicio.
/// </summary>
public class FakeConsultorRepository : IConsultorRepository
{
    private readonly List<Consultor> _consultores = new();
    private int _nextId = 1;

    public Task<PagedResult<Consultor>> ListAsync(ConsultorListQuery query, CancellationToken ct = default)
    {
        var items = _consultores.ToList();
        return Task.FromResult(PagedResult<Consultor>.Create(items, items.Count, 1, Math.Max(1, items.Count)));
    }

    public Task<Consultor?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_consultores.FirstOrDefault(c => c.Id == id));

    public Task<int> InsertAsync(Consultor consultor, CancellationToken ct = default)
    {
        consultor.Id = _nextId++;
        consultor.FechaCreacion = DateTime.UtcNow;
        _consultores.Add(consultor);
        return Task.FromResult(consultor.Id);
    }

    public Task<bool> UpdateAsync(Consultor consultor, CancellationToken ct = default)
    {
        var index = _consultores.FindIndex(c => c.Id == consultor.Id);
        if (index < 0) return Task.FromResult(false);
        _consultores[index] = consultor;
        return Task.FromResult(true);
    }

    public Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var consultor = _consultores.FirstOrDefault(c => c.Id == id);
        if (consultor is null) return Task.FromResult(false);
        consultor.Activo = false;
        return Task.FromResult(true);
    }

    public Task<bool> ExistsByNameAndAreaAsync(string nombreCompleto, string area, int excludeId = 0, CancellationToken ct = default)
        => Task.FromResult(_consultores.Any(c =>
            c.NombreCompleto == nombreCompleto && c.Area == area && c.Id != excludeId));

    public Task<bool> ExistsByEmailAsync(string email, int excludeId = 0, CancellationToken ct = default)
        => Task.FromResult(_consultores.Any(c => c.Email == email && c.Id != excludeId));
}

public class FakePaqueteRepository : IPaqueteRepository
{
    private readonly List<Paquete> _paquetes = new();
    private int _nextId = 1;

    public Task<PagedResult<Paquete>> ListAsync(PaqueteListQuery query, CancellationToken ct = default)
    {
        var items = _paquetes.ToList();
        return Task.FromResult(PagedResult<Paquete>.Create(items, items.Count, 1, Math.Max(1, items.Count)));
    }

    public Task<Paquete?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_paquetes.FirstOrDefault(p => p.Id == id));

    public Task<int> InsertAsync(Paquete paquete, CancellationToken ct = default)
    {
        paquete.Id = _nextId++;
        paquete.FechaCreacion = DateTime.UtcNow;
        _paquetes.Add(paquete);
        return Task.FromResult(paquete.Id);
    }

    public Task<bool> UpdateAsync(Paquete paquete, CancellationToken ct = default)
    {
        var index = _paquetes.FindIndex(p => p.Id == paquete.Id);
        if (index < 0) return Task.FromResult(false);
        _paquetes[index] = paquete;
        return Task.FromResult(true);
    }

    public Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default)
    {
        var paquete = _paquetes.FirstOrDefault(p => p.Id == id);
        if (paquete is null) return Task.FromResult(false);
        paquete.Activo = false;
        return Task.FromResult(true);
    }
}

public class FakeUsuarioRepository : IUsuarioRepository
{
    private readonly Usuario? _usuario;

    public FakeUsuarioRepository(Usuario? usuario)
    {
        _usuario = usuario;
    }

    public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default)
        => Task.FromResult(_usuario);

    public Task<Usuario?> GetByIdAsync(int id, CancellationToken ct = default)
        => Task.FromResult(_usuario);
}

public class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _tokens = new();
    private int _nextId = 1;

    public Task<int> InsertAsync(RefreshToken token, CancellationToken ct = default)
    {
        token.Id = _nextId++;
        token.CreatedAt = DateTime.UtcNow;
        _tokens.Add(token);
        return Task.FromResult(token.Id);
    }

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct = default)
        => Task.FromResult(_tokens.FirstOrDefault(t => t.TokenHash == tokenHash));

    public Task<bool> RevokeAsync(int id, CancellationToken ct = default)
    {
        var token = _tokens.FirstOrDefault(t => t.Id == id);
        if (token is null || token.RevokedAt is not null) return Task.FromResult(false);
        token.RevokedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public Task<int> RevokeAllByUserAsync(int usuarioId, CancellationToken ct = default)
    {
        var revoked = _tokens.Where(t => t.UsuarioId == usuarioId && t.RevokedAt is null).ToList();
        foreach (var token in revoked)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
        return Task.FromResult(revoked.Count);
    }
}

public class FakeJwtTokenGenerator : IJwtTokenGenerator
{
    public string TokenValue { get; set; } = "fake-token";
    public DateTime Expiry { get; set; } = DateTime.UtcNow.AddHours(1);

    public TokenResult Generate(Usuario user)
        => new(TokenValue, Expiry);
}

public class FakeConsultorPaqueteRepository : IConsultorPaqueteRepository
{
    private readonly List<(int ConsultorId, int PaqueteId)> _assignments = new();

    public void Seed(int consultorId, int paqueteId) => _assignments.Add((consultorId, paqueteId));

    public Task<bool> ExistsAsync(int consultorId, int paqueteId, CancellationToken ct = default)
        => Task.FromResult(_assignments.Contains((consultorId, paqueteId)));

    public Task<int> CountByConsultorAsync(int consultorId, CancellationToken ct = default)
        => Task.FromResult(_assignments.Count(a => a.ConsultorId == consultorId));

    public Task<bool> AssignAsync(int consultorId, int paqueteId, CancellationToken ct = default)
    {
        if (_assignments.Contains((consultorId, paqueteId))) return Task.FromResult(false);
        _assignments.Add((consultorId, paqueteId));
        return Task.FromResult(true);
    }

    public Task<bool> UnassignAsync(int consultorId, int paqueteId, CancellationToken ct = default)
        => Task.FromResult(_assignments.Remove((consultorId, paqueteId)));

    public Task<IReadOnlyList<ConsultorPaqueteItem>> ListByConsultorAsync(int consultorId, CancellationToken ct = default)
    {
        var items = _assignments
            .Where(a => a.ConsultorId == consultorId)
            .Select(a => new ConsultorPaqueteItem(
                PaqueteId: a.PaqueteId,
                Nombre: $"Paquete {a.PaqueteId}",
                Descripcion: null,
                Area: "Area",
                Precio: 100m,
                Activo: true,
                FechaAsignacion: DateTime.UtcNow))
            .ToList() as IReadOnlyList<ConsultorPaqueteItem>;
        return Task.FromResult(items);
    }
}

public class FakeAuditService : IAuditService
{
    public List<AuditContext> Entries { get; } = new();

    public Task RecordAsync(AuditContext context, CancellationToken ct = default)
    {
        Entries.Add(context);
        return Task.CompletedTask;
    }
}