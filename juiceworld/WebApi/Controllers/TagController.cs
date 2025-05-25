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
public class TagController(ITagService tagService) : ControllerBase
{
    private const string ApiBaseName = "Tag";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateTag))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<TagDto>> CreateTag(TagDto tag)
    {
        var result = await tagService.CreateTagAsync(tag);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllTags))]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()
    {
        var result = await tagService.GetAllTagsAsync();
        return Ok(result);
    }

    [HttpGet("{TagId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetTag))]
    public async Task<ActionResult<TagDto>> GetTag(int tagId)
    {
        var result = await tagService.GetTagByIdAsync(tagId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateTag))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<TagDto>> UpdateTag(TagDto tag)
    {
        var result = await tagService.UpdateTagAsync(tag);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{TagId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteTag))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeleteTag(int tagId)
    {
        var result = await tagService.DeleteTagByIdAsync(tagId);
        return result ? Ok() : NotFound();
    }
}