using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Application.Mapping;
using Consultora.Application.Services.Contracts;

namespace Consultora.Application.Services;

/// <summary>
/// Consulta de la bitacora de auditoria con paginacion y filtros.
/// </summary>
public class AuditoriaService : IAuditoriaService
{
    private readonly IAuditoriaRepository _repository;

    public AuditoriaService(IAuditoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<AuditoriaDto>> ListAsync(AuditoriaQuery query, CancellationToken ct = default)
    {
        var result = await _repository.ListAsync(query, ct);
        return PagedResult<AuditoriaDto>.Create(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }
}