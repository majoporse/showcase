using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using JuiceWorld.Entities;
using JuiceWorld.Repositories;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class WishListItemServiceTests
{
    private readonly IWishListItemService _wishListItemService;

    public WishListItemServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var wishListItemRepository = new Repository<WishListItem>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        _wishListItemService = new WishListItemService(wishListItemRepository, mapper);
    }

    [Fact]
    public async Task GetAllWishListItemsAsync_ExactMatch()
    {
        // Arrange
        var wishListItemIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _wishListItemService.GetAllWishListItemsAsync();

        // Assert
        var wishListItemDtos = result.ToList();
        Assert.Equal(wishListItemIdsToRetrieve.Length, wishListItemDtos.Count);
        Assert.All(wishListItemIdsToRetrieve,
            id => Assert.Contains(wishListItemDtos, wishListItem => wishListItem.Id == id));
    }

    [Fact]
    public async Task GetWishListItemByIdAsync_ExactMatch()
    {
        // Arrange
        var wishListItemId = 1;

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
        var wishListItem = new WishListItemDto
        {
            Id = 11,
            UserId = 1,
            ProductId = 1
        };

        // Act
        var result = await _wishListItemService.CreateWishListItemAsync(wishListItem);

        // Assert
        Assert.NotNull(result);
        Assert.True(wishListItem.Id == result.Id && wishListItem.ProductId == result.ProductId &&
                    wishListItem.UserId == result.UserId);
    }

    [Fact]
    public async Task UpdateWishListItemAsync_Simple()
    {
        // Arrange
        var wishListItem = new WishListItemDto
        {
            Id = 1,
            UserId = 1,
            ProductId = 1
        };

        // Act
        var result = await _wishListItemService.UpdateWishListItemAsync(wishListItem);

        // Assert
        Assert.NotNull(result);
        Assert.True(wishListItem.Id == result.Id && wishListItem.ProductId == result.ProductId &&
                    wishListItem.UserId == result.UserId);
    }

    [Fact]
    public async Task DeleteWishListItemByIdAsync_Simple()
    {
        // Arrange
        var wishListItemId = 1;

        // Act
        var result = await _wishListItemService.DeleteWishListItemByIdAsync(wishListItemId);

        // Assert
        Assert.True(result);
    }
}