using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Application.Mapping;
using Consultora.Application.Services.Contracts;

namespace Consultora.Application.Services;

public class ReporteService : IReporteService
{
    private readonly IReporteRepository _repository;

    public ReporteService(IReporteRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PaquetePorAreaDto>> PaquetesPorAreaAsync(PaquetesPorAreaQuery query, CancellationToken ct = default)
    {
        var result = await _repository.PaquetesPorAreaAsync(query, ct);
        return PagedResult<PaquetePorAreaDto>.Create(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }

    public async Task<PagedResult<ConsultorFacturacionDto>> ConsultoresTopFacturacionAsync(
        ConsultoresTopFacturacionQuery query, CancellationToken ct = default)
    {
        var result = await _repository.ConsultoresTopFacturacionAsync(query, ct);
        return PagedResult<ConsultorFacturacionDto>.Create(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }
}