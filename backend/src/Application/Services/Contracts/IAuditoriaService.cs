using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Queries;

namespace Consultora.Application.Services.Contracts;

public interface IAuditoriaService
{
    Task<PagedResult<AuditoriaDto>> ListAsync(AuditoriaQuery query, CancellationToken ct = default);
}