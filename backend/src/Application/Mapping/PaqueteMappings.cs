using Consultora.Application.Dtos;
using Consultora.Domain.Entities;
using Mapster;

namespace Consultora.Application.Mapping;

/// <summary>
/// Mapeo de la entidad Paquete hacia su DTO de salida.
/// </summary>
public static class PaqueteMappings
{
    public static PaqueteDto ToDto(this Paquete entity) => entity.Adapt<PaqueteDto>();
}