using Consultora.Application.Dtos;
using Consultora.Domain.Entities;
using Mapster;

namespace Consultora.Application.Mapping;

/// <summary>
/// Mapeo de la entidad Auditoria hacia su DTO de salida.
/// </summary>
public static class AuditoriaMappings
{
    public static AuditoriaDto ToDto(this Auditoria entity) => entity.Adapt<AuditoriaDto>();
}