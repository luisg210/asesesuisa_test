using Consultora.Application.Ports;
using Consultora.Domain.Entities;
using Consultora.Application.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Consultora.Application.Services;

/// <summary>
/// Registra escrituras en la bitacora. Los fallos de auditoria se registran en
/// el log pero nuncan deben impedir la operacion de negocio.
/// </summary>
public class AuditService : IAuditService
{
    private readonly IAuditoriaRepository _repository;
    private readonly ILogger<AuditService> _logger;

    public AuditService(IAuditoriaRepository repository, ILogger<AuditService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task RecordAsync(AuditContext context, CancellationToken ct = default)
    {
        var entry = new Auditoria
        {
            Usuario = string.IsNullOrWhiteSpace(context.Actor) ? "system" : context.Actor,
            Accion = context.Action,
            Entidad = context.Entity,
            EntidadId = context.EntityId,
            Detalle = context.Detail,
            Ip = context.Ip
        };

        try
        {
            await _repository.InsertAsync(entry, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit entry for {Entity} {Id}",
                entry.Entidad, entry.EntidadId);
        }
    }
}