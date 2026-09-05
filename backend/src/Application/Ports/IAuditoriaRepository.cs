using Consultora.Application.Common;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;

namespace Consultora.Application.Ports;

public interface IAuditoriaRepository
{
    Task InsertAsync(Auditoria auditoria, CancellationToken ct = default);
    Task<PagedResult<Auditoria>> ListAsync(AuditoriaQuery query, CancellationToken ct = default);
}