using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Installers;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using JuiceWorld.Entities;
using JuiceWorld.Repositories;
using JuiceWorld.UnitOfWork;
using TestUtilities.MockedObjects;
using Xunit;
using Assert = Xunit.Assert;

namespace BusinessLayer.Tests.Services;

public class CartItemServiceTests
{
    private readonly ICartItemService _cartItemService;

    public CartItemServiceTests()
    {
        var dbContextOptions = MockedDbContext.GetOptions();
        var dbContext = MockedDbContext.CreateFromOptions(dbContextOptions);
        var crtItemRepository = new Repository<CartItem>(dbContext);
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MapperProfileInstaller>());
        var mapper = config.CreateMapper();
        var orderUnitOfWork = new OrderUnitOfWork(dbContext);
        _cartItemService = new CartItemService(crtItemRepository, orderUnitOfWork, mapper);
    }

    [Fact]
    public async Task GetAllCartItemsAsync_ExactMatch()
    {
        // Arrange
        var crtItemIdsToRetrieve = new[] { 1, 2, 3, 4, 5 };

        // Act
        var result = await _cartItemService.GetAllCartItemsAsync();

        // Assert
        var crtItemDtos = result.ToList();
        Assert.Equal(crtItemIdsToRetrieve.Length, crtItemDtos.Count);
        Assert.All(crtItemIdsToRetrieve, id => Assert.Contains(crtItemDtos, crtItem => crtItem.Id == id));
    }

    [Fact]
    public async Task GetCartItemByIdAsync_ExactMatch()
    {
        // Arrange
        var crtItemId = 1;

        // Act
        var result = await _cartItemService.GetCartItemByIdAsync(crtItemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(crtItemId, result.Id);
    }

    [Fact]
    public async Task CreateCartItemAsync_Simple()
    {
        // Arrange
        var crtItem = new CartItemDto
        {
            Id = 12,
            UserId = 1,
            ProductId = 2,
            Quantity = 1
        };

        // Act
        var result = await _cartItemService.CreateCartItemAsync(crtItem);

        // Assert
        Assert.NotNull(result);
        Assert.True(crtItem.UserId == result.UserId && crtItem.Id == result.Id &&
                    crtItem.ProductId == result.ProductId && crtItem.Quantity == result.Quantity);
    }

    [Fact]
    public async Task UpdateCartItemAsync_Simple()
    {
        // Arrange
        var crtItem = new CartItemDto
        {
            Id = 1,
            UserId = 1,
            ProductId = 2,
            Quantity = 1
        };

        // Act
        var result = await _cartItemService.UpdateCartItemAsync(crtItem);

        // Assert
        Assert.NotNull(result);
        Assert.True(crtItem.UserId == result.UserId && crtItem.Id == result.Id &&
                    crtItem.ProductId == result.ProductId && crtItem.Quantity == result.Quantity);
    }

    [Fact]
    public async Task DeleteCartItemByIdAsync_Simple()
    {
        // Arrange
        var crtItemId = 1;

        // Act
        var result = await _cartItemService.DeleteCartItemByIdAsync(crtItemId);

        // Assert
        Assert.True(result);
    }
}
