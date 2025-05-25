using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using JuiceWorld.QueryObjects;
using JuiceWorld.Repositories;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class AuditTrailServiceTests
{
    private readonly IAuditTrailService _auditTrailService;
    private readonly IMapper _mapper;
    private readonly IRepository<Product> _productRepository;

    public AuditTrailServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var auditTrailRepository = new Repository<AuditTrail>(dbContext);
        _productRepository = new Repository<Product>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        _mapper = config.CreateMapper();
        _auditTrailService =
            new AuditTrailService(auditTrailRepository, new QueryObject<AuditTrail>(dbContext), _mapper);
    }

    [Fact]
    public async Task GetAuditFromEntityFiltered()
    {
        // Arrange
        var userId = 111;
        var product = new Product
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
        var result = await _productRepository.CreateAsync(product, userId);

        var filter = new AuditTrailFilterDto
        {
            EntityName = nameof(Product)
        };

        var auditTrail = (await _auditTrailService.GetAuditTrailsFilteredAsync(filter)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(auditTrail);
        Assert.NotEmpty(auditTrail);
        Assert.All(auditTrail, audit =>
        {
            Assert.Equal(nameof(Product), audit.EntityName);
            Assert.Equal(audit.PrimaryKey, result.Id);
            Assert.Equal(audit.UserId, userId);
        });
    }
}