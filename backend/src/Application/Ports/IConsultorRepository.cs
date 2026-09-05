using Consultora.Application.Common;
using Consultora.Application.Queries;
using Consultora.Domain.Entities;

namespace Consultora.Application.Ports;

public interface IConsultorRepository
{
    Task<PagedResult<Consultor>> ListAsync(ConsultorListQuery query, CancellationToken ct = default);
    Task<Consultor?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> InsertAsync(Consultor consultor, CancellationToken ct = default);
    Task<bool> UpdateAsync(Consultor consultor, CancellationToken ct = default);
    Task<bool> SoftDeleteAsync(int id, CancellationToken ct = default);

    /// <summary>Valida unicidad de NombreCompleto + Area (ignorando un Id en updates).</summary>
    Task<bool> ExistsByNameAndAreaAsync(string nombreCompleto, string area, int excludeId = 0, CancellationToken ct = default);

    /// <summary>Valida unicidad de Email (ignorando un Id en updates).</summary>
    Task<bool> ExistsByEmailAsync(string email, int excludeId = 0, CancellationToken ct = default);
}