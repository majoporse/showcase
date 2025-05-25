using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using JuiceWorld.Entities;
using JuiceWorld.QueryObjects;
using JuiceWorld.Repositories;
using JuiceWorld.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class ProductServiceTests
{
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var productRepository = new Repository<Product>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var cache = new MemoryCache(new MemoryCacheOptions());
        var unitOfwork = new ProductUnitOfWork(dbContext);
        _productService = new ProductService(productRepository, mapper, cache, new QueryObject<Product>(dbContext),
            unitOfwork);
    }

    [Fact]
    public async Task GetAllProductsAsync_ExactMatch()
    {
        // Arrange
        var productIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        var productDtos = result.ToList();
        Assert.Equal(productIdsToRetrieve.Length, productDtos.Count);
        Assert.All(productIdsToRetrieve, id => Assert.Contains(productDtos, product => product.Id == id));
    }

    [Fact]
    public async Task GetProductByIdAsync_ExactMatch()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
    }

    [Fact]
    public async Task CreateProductAsync_Simple()
    {
        // Arrange
        var product = new ProductDto
        {
            Id = 9,
            Category = ProductCategory.Dihydrotestosterone,
            Description = "Test description",
            ManufacturerId = 1,
            Name = "Test product",
            Price = 100,
            UsageType = ProductUsageType.Injectable
        };

        // Act
        var result = await _productService.CreateProductAsync(product);

        // Assert
        Assert.NotNull(result);
        Assert.True(product.Id == result.Id && product.Category == result.Category &&
                    product.Description == result.Description && product.ManufacturerId == result.ManufacturerId &&
                    product.Name == result.Name && product.Price == result.Price &&
                    product.UsageType == result.UsageType);
    }

    [Fact]
    public async Task FilterProductsAsync_ExactMatch()
    {
        // Arrange
        var productFilter = new ProductFilterDto
        {
            NameQuery = "Anastrozole",
            PriceMax = 2399,
            PriceMin = 2399,
            PageIndex = 1,
            PageSize = 5
        };

        // Act
        var result = await _productService.GetProductsFilteredAsync(productFilter);

        // Assert
        var productDtos = result.Entities.ToList();
        Assert.Single(productDtos);
        Assert.All(productDtos, product =>
        {
            Assert.Equal(productFilter.NameQuery, product.Name);
            Assert.True(product.Price <= productFilter.PriceMax);
            Assert.True(product.Price >= productFilter.PriceMin);
        });
    }

    [Fact]
    public async Task UpdateProductAsync_Simple()
    {
        // Arrange
        var product = new ProductDto
        {
            Id = 2,
            Category = ProductCategory.Dihydrotestosterone,
            Description = "Test2 description",
            ManufacturerId = 1,
            Name = "Test product2",
            Price = 1002,
            UsageType = ProductUsageType.Injectable
        };

        // Act
        var result = await _productService.UpdateProductAsync(product, 1);

        // Assert
        Assert.NotNull(result);
        Assert.True(product.Id == result.Id && product.Category == result.Category &&
                    product.Description == result.Description && product.ManufacturerId == result.ManufacturerId &&
                    product.Name == result.Name && product.Price == result.Price &&
                    product.UsageType == result.UsageType);
    }

    [Fact]
    public async Task DeleteProductByIdAsync_Simple()
    {
        // Arrange
        var productId = 1;

        // Act
        var result = await _productService.DeleteProductByIdAsync(productId);

        // Assert
        Assert.True(result);
    }
}
