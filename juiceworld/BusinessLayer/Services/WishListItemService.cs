using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.Repositories;
using JuiceWorld.Entities;

namespace BusinessLayer.Services;

public class WishListItemService(IRepository<WishListItem> wishListItemRepository, IMapper mapper)
    : IWishListItemService
{
    public async Task<WishListItemDto?> CreateWishListItemAsync(WishListItemDto wishListItemDto)
    {
        var newWishListItem = await wishListItemRepository.CreateAsync(mapper.Map<WishListItem>(wishListItemDto));
        return newWishListItem is null ? null : mapper.Map<WishListItemDto>(newWishListItem);
    }

    public async Task<IEnumerable<WishListItemDto>> GetAllWishListItemsAsync()
    {
        var wishListItems = await wishListItemRepository.GetAllAsync();
        return mapper.Map<List<WishListItemDto>>(wishListItems);
    }

    public async Task<IEnumerable<WishListItemDetailDto>> GetWishListItemsByUserIdAsync(int userId)
    {
        var wishListItems =
            await wishListItemRepository.GetByConditionAsync(wli => wli.UserId == userId, nameof(WishListItem.Product));
        return mapper.Map<List<WishListItemDetailDto>>(wishListItems);
    }

    public async Task<WishListItemDto?> GetWishListItemByIdAsync(int id)
    {
        var wishListItem = await wishListItemRepository.GetByIdAsync(id);
        return wishListItem is null ? null : mapper.Map<WishListItemDto>(wishListItem);
    }

    public async Task<WishListItemDetailDto?> GetWishListItemDetailByIdAsync(int id)
    {
        var wishListItem =
            await wishListItemRepository.GetByIdAsync(id, nameof(WishListItem.Product), nameof(WishListItem.User));
        return wishListItem is null ? null : mapper.Map<WishListItemDetailDto>(wishListItem);
    }

    public async Task<bool> IsProductInWishListAsync(int productId, int userId)
    {
        var wishListItem =
            await wishListItemRepository.GetByConditionAsync(wli => wli.ProductId == productId && wli.UserId == userId);
        return wishListItem.Any();
    }

    public async Task<WishListItemDto?> UpdateWishListItemAsync(WishListItemDto wishListItemDto)
    {
        var updatedWishListItem = await wishListItemRepository.UpdateAsync(mapper.Map<WishListItem>(wishListItemDto));
        return updatedWishListItem is null ? null : mapper.Map<WishListItemDto>(updatedWishListItem);
    }

    public Task<bool> DeleteWishListItemByIdAsync(int id)
    {
        return wishListItemRepository.DeleteAsync(id);
    }
}
