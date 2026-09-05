using Consultora.Application.Exceptions;
using Consultora.Application.Mapping;
using Consultora.Application.Ports;
using Consultora.Application.Services.Contracts;

namespace Consultora.Application.Services;

/// <summary>
/// Gestiona la relacion N:N consultor; paquete. Reglas:
/// solo se asignan consultores y paquetes activos, una misma
/// relacion no puede duplicarse (recurso unico por par) y un
/// consultor no puede tener mas de 5 paquetes asignados.
/// </summary>
public class ConsultorPaqueteService : IConsultorPaqueteService
{
    public const int MaxPaquetesPerConsultor = 5;

    private readonly IConsultorPaqueteRepository _repository;
    private readonly IConsultorRepository _consultores;
    private readonly IPaqueteRepository _paquetes;
    private readonly IAuditService _audit;

    public ConsultorPaqueteService(
        IConsultorPaqueteRepository repository,
        IConsultorRepository consultores,
        IPaqueteRepository paquetes,
        IAuditService audit)
    {
        _repository = repository;
        _consultores = consultores;
        _paquetes = paquetes;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Dtos.ConsultorPaqueteDto>> ListByConsultorAsync(int consultorId, CancellationToken ct = default)
    {
        await EnsureConsultorExistsAsync(consultorId, ct);

        var items = await _repository.ListByConsultorAsync(consultorId, ct);
        return items
            .Select(x => x.ToDto())
            .ToList();
    }

    public async Task AssignAsync(int consultorId, int paqueteId, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        var consultor = await GetConsultorAsync(consultorId, ct);
        var paquete = await GetPaqueteAsync(paqueteId, ct);

        if (!consultor.Activo)
        {
            throw new ConflictException("Cannot assign paquetes to an inactive consultant.");
        }

        if (!paquete.Activo)
        {
            throw new ConflictException("Cannot assign an inactive paquete to a consultant.");
        }

        if (await _repository.ExistsAsync(consultorId, paqueteId, ct))
        {
            throw new ConflictException("The paquete is already assigned to this consultant.");
        }

        var assignedCount = await _repository.CountByConsultorAsync(consultorId, ct);
        if (assignedCount >= MaxPaquetesPerConsultor)
        {
            throw new ConflictException(
                $"A consultant can have at most {MaxPaquetesPerConsultor} paquetes assigned.");
        }

        await _repository.AssignAsync(consultorId, paqueteId, ct);
        await _audit.RecordAsync(new AuditContext(
            actor, "ASSIGN", "ConsultorPaquete", paqueteId,
            $"{consultor.NombreCompleto} <- {paquete.Nombre}", ip), ct);
    }

    public async Task UnassignAsync(int consultorId, int paqueteId, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        await GetConsultorAsync(consultorId, ct);

        if (!await _repository.ExistsAsync(consultorId, paqueteId, ct))
        {
            throw new NotFoundException($"Paquete {paqueteId} is not assigned to consultant {consultorId}.");
        }

        await _repository.UnassignAsync(consultorId, paqueteId, ct);
        await _audit.RecordAsync(new AuditContext(
            actor, "UNASSIGN", "ConsultorPaquete", paqueteId,
            $"consultor {consultorId} quita paquete {paqueteId}", ip), ct);
    }

    private async Task<Domain.Entities.Consultor> GetConsultorAsync(int id, CancellationToken ct)
        => await _consultores.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Consultor {id} does not exist.");

    private async Task<Domain.Entities.Paquete> GetPaqueteAsync(int id, CancellationToken ct)
        => await _paquetes.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Paquete {id} does not exist.");

    private async Task EnsureConsultorExistsAsync(int id, CancellationToken ct)
    {
        _ = await GetConsultorAsync(id, ct);
    }
}