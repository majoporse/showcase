using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessLayer.Services;

public class TagService(
    IRepository<Tag> tagRepository,
    IQueryObject<Tag> tagQueryObject,
    IMemoryCache memoryCache,
    IMapper mapper) : ITagService
{
    private readonly string _cacheKeyPrefix = nameof(TagService);
    private string CacheKeyTag(int id) => $"{_cacheKeyPrefix}-tag{id}";
    private string CacheKeyTagAll() => $"{_cacheKeyPrefix}-allTags";
    public async Task<TagDto?> CreateTagAsync(TagDto tagDto)
    {
        var newTag = await tagRepository.CreateAsync(mapper.Map<Tag>(tagDto));
        return newTag is null ? null : mapper.Map<TagDto>(newTag);
    }

    public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
    {
        var cacheKey = CacheKeyTagAll();
        if (!memoryCache.TryGetValue(cacheKey, out List<Tag>? value))
        {
            var tags = await tagRepository.GetAllAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, tags.ToList(), cacheEntryOptions);
            value = tags.ToList();
        }

        return mapper.Map<List<TagDto>>(value);
    }

    public async Task<FilteredResult<TagDto>> GetTagsAsync(TagFilterDto tagFilterDto)
    {
        var query = tagQueryObject
            .Filter(t => tagFilterDto.Name == null || t.Name.ToLower().Contains(tagFilterDto.Name.ToLower()))
            .Paginate(tagFilterDto.PageIndex, tagFilterDto.PageSize)
            .OrderBy(m => m.Id);

        var filteredTags = await query.ExecuteAsync();

        return new FilteredResult<TagDto>
        {
            Entities = mapper.Map<List<TagDto>>(filteredTags.Entities),
            PageIndex = filteredTags.PageIndex,
            TotalPages = filteredTags.TotalPages
        };
    }

    public async Task<TagDto?> GetTagByIdAsync(int id)
    {
        var cacheKey = CacheKeyTag(id);
        if (!memoryCache.TryGetValue(cacheKey, out Tag? value))
        {
            value = await tagRepository.GetByIdAsync(id);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            memoryCache.Set(cacheKey, value, cacheEntryOptions);
        }

        return value is null ? null : mapper.Map<TagDto>(value);
    }

    public async Task<TagDto?> UpdateTagAsync(TagDto tagDto)
    {
        var cacheKey = CacheKeyTag(tagDto.Id);
        memoryCache.Remove(cacheKey);
        var cacheKeyAll = $"{_cacheKeyPrefix}-allTags";
        memoryCache.Remove(cacheKeyAll);
        var updatedTag = await tagRepository.UpdateAsync(mapper.Map<Tag>(tagDto));
        return updatedTag is null ? null : mapper.Map<TagDto>(updatedTag);
    }

    public Task<bool> DeleteTagByIdAsync(int id)
    {
        var cacheKey = CacheKeyTag(id);
        memoryCache.Remove(cacheKey);
        var cacheKeyAll = CacheKeyTagAll();
        memoryCache.Remove(cacheKeyAll);
        return tagRepository.DeleteAsync(id);
    }
}