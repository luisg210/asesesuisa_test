using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Queries;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/reportes")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _service;

    public ReportesController(IReporteService service)
    {
        _service = service;
    }

    /// <summary>
    /// Resume cantidad y valor economico (suma de precios) de paquetes por area.
    /// </summary>
    [HttpGet("paquetes-por-area")]
    public async Task<ActionResult<ApiResponse<PagedResult<PaquetePorAreaDto>>>> PaquetesPorArea(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] string? area,
        [FromQuery] bool? activo,
        CancellationToken ct)
    {
        var query = new PaquetesPorAreaQuery(
            new PageRequest(page, pageSize, sortBy, sortDir), area, activo);

        var result = await _service.PaquetesPorAreaAsync(query, ct);
        return Ok(ApiResponse<PagedResult<PaquetePorAreaDto>>.Ok(result));
    }

    /// <summary>
    /// Consultores ordenados por facturacion estimada
    /// (TarifaHora * 160 horas/mes * ProyectosActivos).
    /// </summary>
    [HttpGet("consultores-top-facturacion")]
    public async Task<ActionResult<ApiResponse<PagedResult<ConsultorFacturacionDto>>>> ConsultoresTopFacturacion(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortDir,
        [FromQuery] string? area,
        [FromQuery] bool? activo,
        CancellationToken ct)
    {
        var query = new ConsultoresTopFacturacionQuery(
            new PageRequest(page, pageSize, sortBy, sortDir), area, activo);

        var result = await _service.ConsultoresTopFacturacionAsync(query, ct);
        return Ok(ApiResponse<PagedResult<ConsultorFacturacionDto>>.Ok(result));
    }
}