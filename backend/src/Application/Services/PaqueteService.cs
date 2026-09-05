using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Application.Validation;
using Consultora.Application.Mapping;
using Consultora.Domain.Entities;
using Consultora.Application.Services.Contracts;
using FluentValidation;

namespace Consultora.Application.Services;

public class PaqueteService : IPaqueteService
{
    private readonly IPaqueteRepository _repository;
    private readonly IValidator<PaqueteCreateRequest> _createValidator;
    private readonly IValidator<PaqueteUpdateRequest> _updateValidator;
    private readonly IAuditService? _audit;

    public PaqueteService(
        IPaqueteRepository repository,
        IValidator<PaqueteCreateRequest> createValidator,
        IValidator<PaqueteUpdateRequest> updateValidator)
        : this(repository, createValidator, updateValidator, audit: null)
    {
    }

    public PaqueteService(
        IPaqueteRepository repository,
        IValidator<PaqueteCreateRequest> createValidator,
        IValidator<PaqueteUpdateRequest> updateValidator,
        IAuditService? audit)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _audit = audit;
    }

    public async Task<PaqueteDto> CreateAsync(PaqueteCreateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        var entity = new Paquete
        {
            Nombre = request.Nombre.Trim(),
            Descripcion = request.Descripcion?.Trim(),
            Area = request.Area.Trim(),
            Precio = request.Precio,
            Activo = true
        };

        var id = await _repository.InsertAsync(entity, ct);
        await AuditAsync(actor, "CREATE", "Paquete", id, entity.Nombre, ip, ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<PaqueteDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Paquete {id} does not exist.");
        return entity.ToDto();
    }

    public async Task<PagedResult<PaqueteDto>> ListAsync(PaqueteListQuery query, CancellationToken ct = default)
    {
        var result = await _repository.ListAsync(query, ct);
        return PagedResult<PaqueteDto>.Create(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }

    public async Task<PaqueteDto> UpdateAsync(int id, PaqueteUpdateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var existing = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Paquete {id} does not exist.");

        existing.Nombre = request.Nombre.Trim();
        existing.Descripcion = request.Descripcion?.Trim();
        existing.Area = request.Area.Trim();
        existing.Precio = request.Precio;
        existing.Activo = request.Activo;

        await _repository.UpdateAsync(existing, ct);
        await AuditAsync(actor, "UPDATE", "Paquete", existing.Id, existing.Nombre, ip, ct);
        return existing.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        var existing = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Paquete {id} does not exist.");

        await _repository.SoftDeleteAsync(existing.Id, ct);
        await AuditAsync(actor, "DELETE", "Paquete", existing.Id, existing.Nombre, ip, ct);
    }

    private async Task AuditAsync(string? actor, string action, string entity, int? entityId, string? detail, string? ip, CancellationToken ct)
    {
        if (_audit is null) return;
        await _audit.RecordAsync(new AuditContext(actor, action, entity, entityId, detail, ip), ct);
    }
}