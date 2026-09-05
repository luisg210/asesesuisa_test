using Consultora.Application.Common;
using Consultora.Application.Dtos;

namespace Consultora.Application.Services.Contracts;

public interface IReporteService
{
    Task<PagedResult<PaquetePorAreaDto>> PaquetesPorAreaAsync(Queries.PaquetesPorAreaQuery query, CancellationToken ct = default);
    Task<PagedResult<ConsultorFacturacionDto>> ConsultoresTopFacturacionAsync(Queries.ConsultoresTopFacturacionQuery query, CancellationToken ct = default);
}