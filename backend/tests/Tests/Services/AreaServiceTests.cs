using Consultora.Application.Ports;
using Consultora.Application.Services;

namespace Consultora.Tests.Services;

public class AreaServiceTests
{
    [Fact]
    public async Task GetAllAsync_DevuelveListaDelRepositorio()
    {
        var repository = new FakeAreaRepository(["Finanzas", "Estrategia"]);
        var service = new AreaService(repository);

        var areas = await service.GetAllAsync();

        Assert.Equal(["Finanzas", "Estrategia"], areas);
    }

    private sealed class FakeAreaRepository : IAreaRepository
    {
        private readonly IReadOnlyList<string> _areas;

        public FakeAreaRepository(IReadOnlyList<string> areas)
        {
            _areas = areas;
        }

        public Task<IReadOnlyList<string>> GetAllAsync(CancellationToken ct = default)
            => Task.FromResult(_areas);
    }
}