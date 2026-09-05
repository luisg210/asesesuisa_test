using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/auditoria")]
[Authorize(Roles = "Admin")]
public class AuditoriaController : BaseApiController
{
    private readonly IAuditoriaService _service;

    public AuditoriaController(IAuditoriaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Bitacora de escrituras con paginacion, filtros (entidad, accion, usuario)
    /// y ordenamiento. Solo Admin.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AuditoriaDto>>>> List(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] string? entidad,
        [FromQuery] string? accion,
        [FromQuery] string? usuario,
        CancellationToken ct)
    {
        var query = new AuditoriaQuery(
            new PageRequest(page, pageSize, sortBy, sortDir), entidad, accion, usuario);

        var result = await _service.ListAsync(query, ct);
        return Ok(ApiResponse<PagedResult<AuditoriaDto>>.Ok(result));
    }
}