using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/consultores")]
[Authorize]
public class ConsultoresController : BaseApiController
{
    private readonly IConsultorService _service;

    public ConsultoresController(IConsultorService service)
    {
        _service = service;
    }

    /// <summary>Lista consultores con paginacion, filtros y ordenamiento.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsultorDto>>>> List(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] string? nombre,
        [FromQuery] string? area,
        [FromQuery] bool? activo,
        CancellationToken ct)
    {
        var query = new ConsultorListQuery(
            new PageRequest(page, pageSize, sortBy, sortDir), nombre, area, activo);

        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ConsultorDto>>.Ok(result));
    }

    /// <summary>Obtiene un consultor por Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ConsultorDto>>> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<ConsultorDto>.Ok(result));
    }

    /// <summary>Crea un consultor (solo Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ConsultorDto>>> Create(ConsultorCreateRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct, CurrentUserEmail, ClientIp);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<ConsultorDto>.Ok(result, "Consultor created."));
    }

    /// <summary>Actualiza un consultor (solo Admin).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<ConsultorDto>>> Update(int id, ConsultorUpdateRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct, CurrentUserEmail, ClientIp);
        return Ok(ApiResponse<ConsultorDto>.Ok(result, "Consultor updated."));
    }

    /// <summary>Elimina logicamente un consultor (solo Admin).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct, CurrentUserEmail, ClientIp);
        return Ok(ApiResponse<object>.Ok(null!, "Consultor deleted."));
    }
}