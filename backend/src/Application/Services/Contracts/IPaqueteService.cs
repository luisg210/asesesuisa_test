using Consultora.Application.Common;
using Consultora.Application.Dtos;

namespace Consultora.Application.Services.Contracts;

public interface IPaqueteService
{
    Task<PaqueteDto> CreateAsync(PaqueteCreateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null);
    Task<PaqueteDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<PaqueteDto>> ListAsync(Queries.PaqueteListQuery query, CancellationToken ct = default);
    Task<PaqueteDto> UpdateAsync(int id, PaqueteUpdateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null);
    Task DeleteAsync(int id, CancellationToken ct = default, string? actor = null, string? ip = null);
}