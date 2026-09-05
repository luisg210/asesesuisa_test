namespace Consultora.Application.Ports;

public interface IAreaRepository
{
    Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct = default);
}