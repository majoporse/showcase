using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using Moq;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Stubs;

public class WishListItemServiceStubTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IRepository<WishListItem>> _wishListItemRepositoryMock;

    private readonly List<WishListItem> _wishListItems = new()
    {
        new WishListItem { Id = 1, ProductId = 1, UserId = 1 },
        new WishListItem { Id = 2, ProductId = 2, UserId = 2 }
    };

    private readonly IWishListItemService _wishListItemService;

    public WishListItemServiceStubTests()
    {
        // Initialize mock repository
        _wishListItemRepositoryMock = new Mock<IRepository<WishListItem>>();

        // Configure AutoMapper
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        _mapper = config.CreateMapper();

        // Initialize the service with the mocked repository and mapper
        _wishListItemService = new WishListItemService(_wishListItemRepositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllWishListItemsAsync_ExactMatch()
    {
        // Arrange
        _wishListItemRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_wishListItems);

        // Act
        var result = await _wishListItemService.GetAllWishListItemsAsync();

        // Assert
        var wishListItemDtos = result.ToList();
        Assert.Equal(_wishListItems.Count, wishListItemDtos.Count);
        Assert.All(_wishListItems, item => Assert.Contains(wishListItemDtos, dto => dto.Id == item.Id));
    }

    [Fact]
    public async Task GetWishListItemByIdAsync_ExactMatch()
    {
        // Arrange
        var wishListItemId = 1;
        _wishListItemRepositoryMock.Setup(repo => repo.GetByIdAsync(wishListItemId)).ReturnsAsync(_wishListItems[0]);

        // Act
        var result = await _wishListItemService.GetWishListItemByIdAsync(wishListItemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wishListItemId, result.Id);
    }

    [Fact]
    public async Task CreateWishListItemAsync_Simple()
    {
        // Arrange
        var wishListItemDto = new WishListItemDto { Id = 3, ProductId = 3, UserId = 1 };
        var wishListItem = _mapper.Map<WishListItem>(wishListItemDto);
        _wishListItemRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<WishListItem>(), null, true))
            .ReturnsAsync(wishListItem);

        // Act
        var result = await _wishListItemService.CreateWishListItemAsync(wishListItemDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wishListItemDto.Id, result.Id);
        Assert.Equal(wishListItemDto.ProductId, result.ProductId);
        Assert.Equal(wishListItemDto.UserId, result.UserId);
    }

    [Fact]
    public async Task UpdateWishListItemAsync_Simple()
    {
        // Arrange
        var wishListItemDto = new WishListItemDto { Id = 1, ProductId = 3, UserId = 1 };
        var wishListItem = _mapper.Map<WishListItem>(wishListItemDto);
        _wishListItemRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<WishListItem>(), null, true))
            .ReturnsAsync(wishListItem);

        // Act
        var result = await _wishListItemService.UpdateWishListItemAsync(wishListItemDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wishListItemDto.Id, result.Id);
        Assert.Equal(wishListItemDto.ProductId, result.ProductId);
        Assert.Equal(wishListItemDto.UserId, result.UserId);
    }

    [Fact]
    public async Task DeleteWishListItemByIdAsync_Simple()
    {
        // Arrange
        var wishListItemId = 1;
        _wishListItemRepositoryMock.Setup(repo => repo.DeleteAsync(wishListItemId, null, true)).ReturnsAsync(true);

        // Act
        var result = await _wishListItemService.DeleteWishListItemByIdAsync(wishListItemId);

        // Assert
        Assert.True(result);
    }
}
