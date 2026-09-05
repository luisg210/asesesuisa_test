using Consultora.Application.Common;
using Consultora.Application.Dtos;

namespace Consultora.Application.Services.Contracts;

public interface IConsultorService
{
    Task<ConsultorDto> CreateAsync(ConsultorCreateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null);
    Task<ConsultorDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PagedResult<ConsultorDto>> ListAsync(Queries.ConsultorListQuery query, CancellationToken ct = default);
    Task<ConsultorDto> UpdateAsync(int id, ConsultorUpdateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null);
    Task DeleteAsync(int id, CancellationToken ct = default, string? actor = null, string? ip = null);
}