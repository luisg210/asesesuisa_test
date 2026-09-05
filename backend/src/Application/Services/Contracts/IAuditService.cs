namespace Consultora.Application.Services.Contracts;

public interface IAuditService
{
    Task RecordAsync(AuditContext context, CancellationToken ct = default);
}