using Consultora.Application.Dtos;
using Consultora.Domain.Entities;
using Mapster;

namespace Consultora.Application.Mapping;

/// <summary>
/// Mapeo de la entidad Consultor hacia su DTO de salida.
/// </summary>
public static class ConsultorMappings
{
    public static ConsultorDto ToDto(this Consultor entity) => entity.Adapt<ConsultorDto>();
}