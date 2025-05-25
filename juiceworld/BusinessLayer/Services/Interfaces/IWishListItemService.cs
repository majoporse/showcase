using BusinessLayer.DTOs;

namespace BusinessLayer.Services.Interfaces;

public interface IWishListItemService
{
    Task<WishListItemDto?> CreateWishListItemAsync(WishListItemDto wishListItemDto);
    Task<IEnumerable<WishListItemDto>> GetAllWishListItemsAsync();
    Task<IEnumerable<WishListItemDetailDto>> GetWishListItemsByUserIdAsync(int userId);
    Task<WishListItemDto?> GetWishListItemByIdAsync(int id);
    Task<WishListItemDetailDto?> GetWishListItemDetailByIdAsync(int id);
    Task<bool> IsProductInWishListAsync(int productId, int userId);
    Task<WishListItemDto?> UpdateWishListItemAsync(WishListItemDto wishListItemDto);
    Task<bool> DeleteWishListItemByIdAsync(int id);
}
