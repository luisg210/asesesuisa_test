using Consultora.Application.Ports;
using Consultora.Application.Services.Contracts;

namespace Consultora.Application.Services;

public class AreaService : IAreaService
{
    private readonly IAreaRepository _repository;

    public AreaService(IAreaRepository repository)
    {
        _repository = repository;
    }

    public Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct = default)
        => _repository.GetAllAsync(ct);
}