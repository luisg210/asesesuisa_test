using Consultora.Application.Common;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/areas")]
[Authorize]
public class AreasController : ControllerBase
{
    private readonly IAreaService _service;

    public AreasController(IAreaService service)
    {
        _service = service;
    }

    /// <summary>
    /// Catalogo de areas disponibles (distinct sobre Paquetes y Consultores).
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<string>>>> GetAll(CancellationToken ct)
    {
        var areas = await _service.GetAllAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<string>>.Ok(areas));
    }
}