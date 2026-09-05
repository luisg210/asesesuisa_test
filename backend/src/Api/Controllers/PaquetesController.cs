using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/paquetes")]
[Authorize]
public class PaquetesController : BaseApiController
{
    private readonly IPaqueteService _service;

    public PaquetesController(IPaqueteService service)
    {
        _service = service;
    }

    /// <summary>Lista paquetes con paginacion, filtros y ordenamiento.</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PaqueteDto>>>> List(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] string? nombre,
        [FromQuery] string? area,
        [FromQuery] bool? activo,
        CancellationToken ct)
    {
        var query = new PaqueteListQuery(
            new PageRequest(page, pageSize, sortBy, sortDir), nombre, area, activo);

        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<PaqueteDto>>.Ok(result));
    }

    /// <summary>Obtiene un paquete por Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PaqueteDto>>> GetById(int id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return Ok(ApiResponse<PaqueteDto>.Ok(result));
    }

    /// <summary>Crea un paquete (solo Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaqueteDto>>> Create(PaqueteCreateRequest request, CancellationToken ct)
    {
        var result = await _service.CreateAsync(request, ct, CurrentUserEmail, ClientIp);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<PaqueteDto>.Ok(result, "Paquete created."));
    }

    /// <summary>Actualiza un paquete (solo Admin).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<PaqueteDto>>> Update(int id, PaqueteUpdateRequest request, CancellationToken ct)
    {
        var result = await _service.UpdateAsync(id, request, ct, CurrentUserEmail, ClientIp);
        return Ok(ApiResponse<PaqueteDto>.Ok(result, "Paquete updated."));
    }

    /// <summary>Elimina logicamente un paquete (solo Admin).</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct, CurrentUserEmail, ClientIp);
        return Ok(ApiResponse<object>.Ok(null!, "Paquete deleted."));
    }
}