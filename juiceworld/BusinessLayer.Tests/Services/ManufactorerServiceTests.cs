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

public class ManufacturerServiceTests
{
    private readonly IManufacturerService _manufacturerService;

    public ManufacturerServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var manufacturerRepository = new Repository<Manufacturer>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        var manufacturerQueryObject = new QueryObject<Manufacturer>(dbContext);
        var cache = new MemoryCache(new MemoryCacheOptions());
        _manufacturerService = new ManufacturerService(manufacturerRepository, manufacturerQueryObject, cache, mapper);
    }

    [Fact]
    public async Task GetAllManufacturersAsync_ExactMatch()
    {
        // Arrange
        var manufacturerIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _manufacturerService.GetAllManufacturersAsync();

        // Assert
        var manufacturerDtos = result.ToList();
        Assert.Equal(manufacturerIdsToRetrieve.Length, manufacturerDtos.Count);
        Assert.All(manufacturerIdsToRetrieve,
            id => Assert.Contains(manufacturerDtos, manufacturer => manufacturer.Id == id));
    }

    [Fact]
    public async Task GetManufacturerByIdAsync_ExactMatch()
    {
        // Arrange
        var manufacturerId = 1;

        // Act
        var result = await _manufacturerService.GetManufacturerByIdAsync(manufacturerId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(manufacturerId, result.Id);
    }

    [Fact]
    public async Task CreateManufacturerAsync_Simple()
    {
        // Arrange
        var manufacturer = new ManufacturerDto
        {
            Id = 10,
            Name = "MediPharma123"
        };

        // Act
        var result = await _manufacturerService.CreateManufacturerAsync(manufacturer);

        // Assert
        Assert.NotNull(result);
        Assert.True(manufacturer.Id == result.Id && manufacturer.Name == result.Name);
    }

    [Fact]
    public async Task UpdateManufacturerAsync_Simple()
    {
        // Arrange
        var manufacturer = new ManufacturerDto
        {
            Id = 1,
            Name = "MediPharma123"
        };

        // Act
        var result = await _manufacturerService.UpdateManufacturerAsync(manufacturer);

        // Assert
        Assert.NotNull(result);
        Assert.True(manufacturer.Id == result.Id && manufacturer.Name == result.Name);
    }

    [Fact]
    public async Task DeleteManufacturerByIdAsync_Simple()
    {
        // Arrange
        var manufacturerId = 1;

        // Act
        var result = await _manufacturerService.DeleteManufacturerByIdAsync(manufacturerId);

        // Assert
        Assert.True(result);
    }
}