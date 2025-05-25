using System.Linq.Expressions;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Moq;
using Xunit;

namespace BusinessLayer.Tests.Stubs;

public class AuditTrailServiceStubTests
{
    private readonly Mock<IRepository<AuditTrail>> _auditTrailRepositoryMock;
    private readonly IAuditTrailService _auditTrailService;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IQueryObject<AuditTrail>> _queryObjectMock;

    public AuditTrailServiceStubTests()
    {
        // Mock the IRepository<AuditTrail>
        _auditTrailRepositoryMock = new Mock<IRepository<AuditTrail>>();

        // Mock the IQueryObject<AuditTrail>
        _queryObjectMock = new Mock<IQueryObject<AuditTrail>>();

        // Mock the IMapper
        _mapperMock = new Mock<IMapper>();

        // Initialize the service with the mocks
        _auditTrailService = new AuditTrailService(
            _auditTrailRepositoryMock.Object,
            _queryObjectMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAuditTrailsFilteredAsync_ValidFilter_ReturnsMappedAuditTrails()
    {
        // Arrange
        var filterDto = new AuditTrailFilterDto
        {
            TrailType = "Create",
            TimestampFrom = DateTime.UtcNow.AddDays(-1),
            UserId = 1,
            PageIndex = 1,
            PageSize = 10
        };

        var auditTrails = new FilteredResult<AuditTrail>
        {
            Entities = new List<AuditTrail>
            {
                new()
                {
                    Id = 1,
                    TrailType = TrailType.Create,
                    CreatedAt = DateTime.UtcNow,
                    EntityName = "Product",
                    PrimaryKey = 123,
                    UserId = 1,
                    ChangedColumns = new List<string> { "Name", "Price" }
                }
            },
            PageIndex = 1,
            TotalPages = 1
        };


        var auditTrailDto = new AuditTrailDto
        {
            Id = 1,
            TrailType = TrailType.Create,
            CreatedAt = DateTime.UtcNow,
            EntityName = "Product",
            PrimaryKey = 123,
            UserId = 1,
            ChangedColumns = new List<string> { "Name", "Price" }
        };

        // Setup the mocks
        _queryObjectMock.Setup(qo => qo.Filter(It.IsAny<Expression<Func<AuditTrail, bool>>>()))
            .Returns(_queryObjectMock.Object);

        _queryObjectMock.Setup(qo => qo.OrderBy(It.IsAny<Expression<Func<AuditTrail, DateTime>>>(), false))
            .Returns(_queryObjectMock.Object);

        _queryObjectMock.Setup(qo => qo.ExecuteAsync())
            .ReturnsAsync(auditTrails);

        _queryObjectMock.Setup(qo => qo.Paginate(1, 10))
            .Returns(_queryObjectMock.Object);

        _mapperMock.Setup(m => m.Map<List<AuditTrailDto>>(auditTrails.Entities))
            .Returns(new List<AuditTrailDto> { auditTrailDto });

        // Act
        var result = (await _auditTrailService.GetAuditTrailsFilteredAsync(filterDto)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(auditTrails.Entities.First().Id, result.First().Id);
        Assert.Equal(auditTrails.Entities.First().TrailType, result.First().TrailType);
    }


    [Fact]
    public async Task GetAuditTrailsFilteredAsync_InvalidFilter_ReturnsEmptyList()
    {
        // Arrange
        var filterDto = new AuditTrailFilterDto
        {
            TrailType = "InvalidType" // Invalid filter that doesn't match any audit trail
        };

        var auditTrails = new List<AuditTrail>();

        // Setup the mocks
        _queryObjectMock.Setup(qo => qo.Filter(It.IsAny<Expression<Func<AuditTrail, bool>>>()))
            .Returns(_queryObjectMock.Object);

        _queryObjectMock.Setup(qo => qo.ExecuteAsync())
            .ReturnsAsync(new FilteredResult<AuditTrail>());

        _queryObjectMock.Setup(qo => qo.OrderBy(It.IsAny<Expression<Func<AuditTrail, DateTime>>>(), false))
            .Returns(_queryObjectMock.Object);

        _mapperMock.Setup(m => m.Map<List<AuditTrailDto>>(auditTrails))
            .Returns(new List<AuditTrailDto>());

        // Act
        var result = await _auditTrailService.GetAuditTrailsFilteredAsync(filterDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result); // We expect an empty list for invalid filter
    }

    [Fact]
    public async Task GetAuditTrailByIdAsync_ExistingId_ReturnsMappedAuditTrail()
    {
        // Arrange
        var auditTrail = new AuditTrail
        {
            Id = 1,
            TrailType = TrailType.Update,
            CreatedAt = DateTime.UtcNow,
            EntityName = "Product",
            PrimaryKey = 123,
            UserId = 1,
            ChangedColumns = new List<string> { "Name" }
        };

        var auditTrailDto = new AuditTrailDto
        {
            Id = 1,
            TrailType = TrailType.Update,
            CreatedAt = DateTime.UtcNow,
            EntityName = "Product",
            PrimaryKey = 123,
            UserId = 1,
            ChangedColumns = new List<string> { "Name" }
        };

        // Setup the mocks
        _auditTrailRepositoryMock.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(auditTrail);

        _mapperMock.Setup(m => m.Map<AuditTrailDto>(auditTrail))
            .Returns(auditTrailDto);

        // Act
        var result = await _auditTrailService.GetAuditTrailByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(auditTrail.Id, result.Id);
        Assert.Equal(auditTrail.TrailType, result.TrailType);
    }

    [Fact]
    public async Task GetAuditTrailByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        _auditTrailRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((AuditTrail)null);

        // Act
        var result = await _auditTrailService.GetAuditTrailByIdAsync(999); // ID that doesn't exist

        // Assert
        Assert.Null(result);
    }
}