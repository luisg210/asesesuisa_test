using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Mapster;

namespace Consultora.Application.Mapping;

/// <summary>
/// Mapeo del paquete asignado (resultado de consulta) hacia su DTO de salida.
/// </summary>
public static class ConsultorPaqueteMappings
{
    public static ConsultorPaqueteDto ToDto(this ConsultorPaqueteItem item) => item.Adapt<ConsultorPaqueteDto>();
}