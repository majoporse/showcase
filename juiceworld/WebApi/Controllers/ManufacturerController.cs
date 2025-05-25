using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
public class ManufacturerController(IManufacturerService manufacturerService) : ControllerBase
{
    private const string ApiBaseName = "Manufacturer";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateManufacturer))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ManufacturerDto>> CreateManufacturer(ManufacturerDto manufacturer)
    {
        var result = await manufacturerService.CreateManufacturerAsync(manufacturer);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllManufacturers))]
    public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetAllManufacturers()
    {
        var result = await manufacturerService.GetAllManufacturersAsync();
        return Ok(result);
    }

    [HttpGet("{manufacturerId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetManufacturer))]
    public async Task<ActionResult<ManufacturerDto>> GetManufacturer(int manufacturerId)
    {
        var result = await manufacturerService.GetManufacturerByIdAsync(manufacturerId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateManufacturer))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ManufacturerDto>> UpdateManufacturer(ManufacturerDto manufacturer)
    {
        var result = await manufacturerService.UpdateManufacturerAsync(manufacturer);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{manufacturerId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteManufacturer))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeleteManufacturer(int manufacturerId)
    {
        var result = await manufacturerService.DeleteManufacturerByIdAsync(manufacturerId);
        return result ? Ok() : NotFound();
    }
}