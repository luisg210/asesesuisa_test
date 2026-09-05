using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Mapster;

namespace Consultora.Application.Mapping;

/// <summary>
/// Mapeo de los resultados de reportes hacia sus DTOs de salida.
/// </summary>
public static class ReporteMappings
{
    public static PaquetePorAreaDto ToDto(this PaquetePorArea item) => item.Adapt<PaquetePorAreaDto>();

    public static ConsultorFacturacionDto ToDto(this ConsultorFacturacion item) => item.Adapt<ConsultorFacturacionDto>();
}