using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Exceptions;
using Consultora.Application.Ports;
using Consultora.Application.Queries;
using Consultora.Application.Mapping;
using Consultora.Domain.Entities;
using Consultora.Application.Services.Contracts;
using FluentValidation;

namespace Consultora.Application.Services;

public class ConsultorService : IConsultorService
{
    private readonly IConsultorRepository _repository;
    private readonly IValidator<ConsultorCreateRequest> _createValidator;
    private readonly IValidator<ConsultorUpdateRequest> _updateValidator;
    private readonly IAuditService? _audit;

    public ConsultorService(
        IConsultorRepository repository,
        IValidator<ConsultorCreateRequest> createValidator,
        IValidator<ConsultorUpdateRequest> updateValidator)
        : this(repository, createValidator, updateValidator, audit: null)
    {
    }

    public ConsultorService(
        IConsultorRepository repository,
        IValidator<ConsultorCreateRequest> createValidator,
        IValidator<ConsultorUpdateRequest> updateValidator,
        IAuditService? audit)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _audit = audit;
    }

    public async Task<ConsultorDto> CreateAsync(ConsultorCreateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);

        await EnsureUniqueAsync(request.NombreCompleto, request.Email, request.Area, ct);

        var entity = new Consultor
        {
            NombreCompleto = request.NombreCompleto.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Area = request.Area.Trim(),
            TarifaHora = request.TarifaHora,
            ProyectosActivos = request.ProyectosActivos,
            Activo = true
        };

        var id = await _repository.InsertAsync(entity, ct);
        await AuditAsync(actor, "CREATE", "Consultor", id, entity.NombreCompleto, ip, ct);
        return await GetByIdAsync(id, ct);
    }

    public async Task<ConsultorDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Consultor {id} does not exist.");
        return entity.ToDto();
    }

    public async Task<PagedResult<ConsultorDto>> ListAsync(ConsultorListQuery query, CancellationToken ct = default)
    {
        var result = await _repository.ListAsync(query, ct);
        return PagedResult<ConsultorDto>.Create(
            result.Items.Select(x => x.ToDto()).ToList(),
            result.TotalCount,
            result.Page,
            result.PageSize);
    }

    public async Task<ConsultorDto> UpdateAsync(int id, ConsultorUpdateRequest request, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        await _updateValidator.ValidateAndThrowAsync(request, ct);

        var existing = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Consultor {id} does not exist.");

        var email = request.Email.Trim().ToLowerInvariant();
        var nombre = request.NombreCompleto.Trim();
        var area = request.Area.Trim();

        await EnsureUniqueAsync(nombre, email, area, ct, excludeId: id);

        existing.NombreCompleto = nombre;
        existing.Email = email;
        existing.Area = area;
        existing.TarifaHora = request.TarifaHora;
        existing.ProyectosActivos = request.ProyectosActivos;
        existing.Activo = request.Activo;

        await _repository.UpdateAsync(existing, ct);
        await AuditAsync(actor, "UPDATE", "Consultor", existing.Id, existing.NombreCompleto, ip, ct);
        return existing.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default, string? actor = null, string? ip = null)
    {
        var existing = await _repository.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"Consultor {id} does not exist.");

        await _repository.SoftDeleteAsync(existing.Id, ct);
        await AuditAsync(actor, "DELETE", "Consultor", existing.Id, existing.NombreCompleto, ip, ct);
    }

    private async Task AuditAsync(string? actor, string action, string entity, int? entityId, string? detail, string? ip, CancellationToken ct)
    {
        if (_audit is null) return;
        await _audit.RecordAsync(new AuditContext(actor, action, entity, entityId, detail, ip), ct);
    }

    private async Task EnsureUniqueAsync(string nombreCompleto, string email, string area, CancellationToken ct, int excludeId = 0)
    {
        if (await _repository.ExistsByNameAndAreaAsync(nombreCompleto.Trim(), area.Trim(), excludeId, ct))
        {
            throw new ConflictException("A consultant with the same name and area already exists.");
        }

        if (await _repository.ExistsByEmailAsync(email.Trim().ToLowerInvariant(), excludeId, ct))
        {
            throw new ConflictException("A consultant with the same email already exists.");
        }
    }
}