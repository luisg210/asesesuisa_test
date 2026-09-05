using Consultora.Application.Common;
using Consultora.Application.Queries;

namespace Consultora.Application.Ports;

public interface IReporteRepository
{
    Task<PagedResult<PaquetePorArea>> PaquetesPorAreaAsync(PaquetesPorAreaQuery query, CancellationToken ct = default);
    Task<PagedResult<ConsultorFacturacion>> ConsultoresTopFacturacionAsync(ConsultoresTopFacturacionQuery query, CancellationToken ct = default);
}