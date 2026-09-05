using Consultora.Application.Queries;

namespace Consultora.Application.Ports;

public interface IConsultorPaqueteRepository
{
    Task<bool> ExistsAsync(int consultorId, int paqueteId, CancellationToken ct = default);
    Task<int> CountByConsultorAsync(int consultorId, CancellationToken ct = default);
    Task<bool> AssignAsync(int consultorId, int paqueteId, CancellationToken ct = default);
    Task<bool> UnassignAsync(int consultorId, int paqueteId, CancellationToken ct = default);
    Task<IReadOnlyList<ConsultorPaqueteItem>> ListByConsultorAsync(int consultorId, CancellationToken ct = default);
}