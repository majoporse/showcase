using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using JuiceWorld.UnitOfWork;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Stubs;

public class ProductServiceStubTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepository<Product>> _productRepositoryMock;

    private readonly List<Product> _products = new()
    {
        new Product
        {
            Id = 1,
            Name = "Test Product 1",
            Price = 100m,
            Description = "Description 1",
            Category = ProductCategory.Testosterone,
            ManufacturerId = 1
        },
        new Product
        {
            Id = 2,
            Name = "Test Product 2",
            Price = 150m,
            Description = "Description 2",
            Category = ProductCategory.Testosterone,
            ManufacturerId = 2
        }
    };

    private readonly IProductService _productService;
    private readonly Mock<IRepository<Tag>> _tagRepositoryMock;
    private readonly Mock<ProductUnitOfWork> _unitofWork;

    public ProductServiceStubTests()
    {
        // Initialize mocks
        _productRepositoryMock = new Mock<IRepository<Product>>();
        Mock<IQueryObject<Product>> queryObjectMock = new();

        // Configure AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        _mapper = config.CreateMapper();
        var cache = new MemoryCache(new MemoryCacheOptions());
        // Initialize the service
        _tagRepositoryMock = new Mock<IRepository<Tag>>();
        _unitofWork = new Mock<ProductUnitOfWork>(_productRepositoryMock.Object, _tagRepositoryMock.Object);
        _productService = new ProductService(_productRepositoryMock.Object, _mapper, cache, queryObjectMock.Object,
            _unitofWork.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ExactMatch()
    {
        // Arrange
        _productRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_products);

        // Act
        var result = await _productService.GetAllProductsAsync();

        // Assert
        var productDtos = result.ToList();
        Assert.Equal(_products.Count, productDtos.Count);
        Assert.All(_products, product => Assert.Contains(productDtos, dto => dto.Id == product.Id));
    }

    [Fact]
    public async Task GetProductByIdAsync_ExactMatch()
    {
        // Arrange
        var productId = 1;
        _productRepositoryMock.Setup(repo => repo.GetByIdAsync(productId, nameof(Product.Tags)))
            .ReturnsAsync(_products[0]);

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
        var productDto = new ProductDto
        {
            Id = 3,
            Name = "New Product",
            Price = 200m,
            Description = "New Description",
            Category = ProductCategory.Testosterone,
            ManufacturerId = 3
        };

        var product = _mapper.Map<Product>(productDto);
        _productRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Product>(), null, true)).ReturnsAsync(product);

        // Act
        var result = await _productService.CreateProductAsync(productDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productDto.Id, result.Id);
        Assert.Equal(productDto.Name, result.Name);
        Assert.Equal(productDto.Price, result.Price);
        Assert.Equal(productDto.Description, result.Description);
        Assert.Equal(productDto.Category, result.Category);
        Assert.Equal(productDto.ManufacturerId, result.ManufacturerId);
    }

    [Fact]
    public async Task DeleteProductByIdAsync_Simple()
    {
        // Arrange
        var productId = 1;
        _productRepositoryMock.Setup(repo => repo.DeleteAsync(productId, null, true)).ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteProductByIdAsync(productId);

        // Assert
        Assert.True(result);
    }
}
