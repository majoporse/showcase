using BusinessLayer.DTOs;
using Infrastructure.QueryObjects;

namespace BusinessLayer.Services.Interfaces;

public interface ITagService
{
    Task<TagDto?> CreateTagAsync(TagDto tagDto);
    Task<IEnumerable<TagDto>> GetAllTagsAsync();
    Task<FilteredResult<TagDto>> GetTagsAsync(TagFilterDto tagFilterDto);
    Task<TagDto?> GetTagByIdAsync(int id);
    Task<TagDto?> UpdateTagAsync(TagDto tagDto);
    Task<bool> DeleteTagByIdAsync(int id);
}