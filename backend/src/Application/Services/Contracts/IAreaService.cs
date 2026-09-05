namespace Consultora.Application.Services.Contracts;

public interface IAreaService
{
    Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct = default);
}