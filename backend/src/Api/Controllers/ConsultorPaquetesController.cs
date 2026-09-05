using Consultora.Application.Common;
using Consultora.Application.Dtos;
using Consultora.Application.Services;
using Consultora.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Consultora.Api.Controllers;

[ApiController]
[Route("api/v1/consultores/{consultorId:int}/paquetes")]
[Authorize]
public class ConsultorPaquetesController : BaseApiController
{
    private readonly IConsultorPaqueteService _service;

    public ConsultorPaquetesController(IConsultorPaqueteService service)
    {
        _service = service;
    }

    /// <summary>Lista los paquetes asignados a un consultor (Admin/User).</summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>>> ListByConsultor(
        int consultorId, CancellationToken ct)
    {
        var result = await _service.ListByConsultorAsync(consultorId, ct);
        return Ok(ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>.Ok(result));
    }

    /// <summary>Asigna un paquete a un consultor (solo Admin).</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>>> Assign(
        int consultorId, AsignarPaqueteRequest request, CancellationToken ct)
    {
        await _service.AssignAsync(consultorId, request.PaqueteId, ct, CurrentUserEmail, ClientIp);
        var result = await _service.ListByConsultorAsync(consultorId, ct);
        return Ok(ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>.Ok(result, "Paquete asignado."));
    }

    /// <summary>Quita la asignacion de un paquete a un consultor (solo Admin).</summary>
    [HttpDelete("{paqueteId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>>> Unassign(
        int consultorId, int paqueteId, CancellationToken ct)
    {
        await _service.UnassignAsync(consultorId, paqueteId, ct, CurrentUserEmail, ClientIp);
        var result = await _service.ListByConsultorAsync(consultorId, ct);
        return Ok(ApiResponse<IReadOnlyList<ConsultorPaqueteDto>>.Ok(result, "Paquete desasignado."));
    }
}