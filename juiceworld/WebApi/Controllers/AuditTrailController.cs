using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin))]
public class AuditTrailController(IAuditTrailService auditTrailService) : ControllerBase
{
    private const string ApiBaseName = "AuditTrail";

    [HttpGet("{id:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetAuditTrail))]
    public async Task<ActionResult<AuditTrailDto>> GetAuditTrail(int id)
    {
        var auditTrail = await auditTrailService.GetAuditTrailByIdAsync(id);
        return auditTrail is null ? NotFound() : Ok(auditTrail);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllAuditTrails))]
    public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetAllAuditTrails(
        [FromQuery] AuditTrailFilterDto filter)
    {
        var auditTrails = await auditTrailService.GetAuditTrailsFilteredAsync(filter);
        return Ok(auditTrails);
    }
}