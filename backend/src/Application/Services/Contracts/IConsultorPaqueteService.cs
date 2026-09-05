namespace Consultora.Application.Services.Contracts;

public interface IConsultorPaqueteService
{
    Task<IReadOnlyList<Dtos.ConsultorPaqueteDto>> ListByConsultorAsync(int consultorId, CancellationToken ct = default);
    Task AssignAsync(int consultorId, int paqueteId, CancellationToken ct = default, string? actor = null, string? ip = null);
    Task UnassignAsync(int consultorId, int paqueteId, CancellationToken ct = default, string? actor = null, string? ip = null);
}