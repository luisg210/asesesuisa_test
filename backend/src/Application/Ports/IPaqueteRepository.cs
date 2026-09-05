using Consultora.Application.Common;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;

namespace Consultora.Application.Ports;

public interface IPaqueteRepository
{
    Task<PagedResult<Paquete>> ListAsync(PaqueteListQuery query, CancellationToken ct = default);
    Task<Paquete?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> InsertAsync(Paquete paquete, CancellationToken ct = default);
    Task<bool> UpdateAsync(Paquete paquete, CancellationToken ct = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default);
}