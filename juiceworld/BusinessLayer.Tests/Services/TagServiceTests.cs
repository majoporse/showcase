using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using JuiceWorld.Entities;
using JuiceWorld.QueryObjects;
using JuiceWorld.Repositories;
using Microsoft.Extensions.Caching.Memory;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class TagServiceTests
{
    private readonly ITagService _tagService;

    public TagServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var tagRepository = new Repository<Tag>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        var tagQueryObject = new QueryObject<Tag>(dbContext);
        var cache = new MemoryCache(new MemoryCacheOptions());
        _tagService = new TagService(tagRepository, tagQueryObject, cache, mapper);
    }

    [Fact]
    public async Task GetAllTagsAsync_ExactMatch()
    {
        // Arrange
        var tagIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _tagService.GetAllTagsAsync();

        // Assert
        var tagDtos = result.ToList();
        Assert.Equal(tagIdsToRetrieve.Length, tagDtos.Count);
        Assert.All(tagIdsToRetrieve, id => Assert.Contains(tagDtos, tag => tag.Id == id));
    }

    [Fact]
    public async Task GetTagByIdAsync_ExactMatch()
    {
        // Arrange
        var tagIdToRetrieve = 1;

        // Act
        var result = await _tagService.GetTagByIdAsync(tagIdToRetrieve);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tagIdToRetrieve, result.Id);
    }

    [Fact]
    public async Task CreateTagAsync_Simple()
    {
        // Arrange
        var tagDto = new TagDto
        {
            Name = "TestTag",
            ColorHex = "#ff0000"
        };

        // Act
        var result = await _tagService.CreateTagAsync(tagDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tagDto.Name, result.Name);
        Assert.Equal(tagDto.ColorHex, result.ColorHex);
    }

    [Fact]
    public async Task UpdateTagAsync_Simple()
    {
        // Arrange
        var tagDto = new TagDto
        {
            Id = 1,
            Name = "UpdatedTag",
            ColorHex = "#00ff00"
        };

        // Act
        var result = await _tagService.UpdateTagAsync(tagDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tagDto.Id, result.Id);
        Assert.Equal(tagDto.Name, result.Name);
        Assert.Equal(tagDto.ColorHex, result.ColorHex);
    }

    [Fact]
    public async Task DeleteTagByIdAsync_Simple()
    {
        // Arrange
        var tagIdToDelete = 1;

        // Act
        var result = await _tagService.DeleteTagByIdAsync(tagIdToDelete);

        // Assert
        Assert.True(result);
    }
}