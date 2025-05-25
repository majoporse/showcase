using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.Repositories;
using JuiceWorld.Entities;
using JuiceWorld.UnitOfWork;

namespace BusinessLayer.Services;

public class CartItemService(
    IRepository<CartItem> cartItemRepository,
    OrderUnitOfWork orderUnitOfWork,
    IMapper mapper)
    : ICartItemService
{
    public async Task<CartItemDto?> CreateCartItemAsync(CartItemDto cartItemDto)
    {
        var newCartItem = await cartItemRepository.CreateAsync(mapper.Map<CartItem>(cartItemDto));
        return newCartItem is null ? null : mapper.Map<CartItemDto>(newCartItem);
    }

    public async Task<IEnumerable<CartItemDto>> GetAllCartItemsAsync()
    {
        var cartItems = await cartItemRepository.GetAllAsync();
        return mapper.Map<List<CartItemDto>>(cartItems);
    }

    public async Task<IEnumerable<CartItemDetailDto>> GetCartItemsByUserIdAsync(int userId)
    {
        var cartItems = await cartItemRepository.GetByConditionAsync(c => c.UserId == userId, nameof(CartItem.Product));
        return mapper.Map<List<CartItemDetailDto>>(cartItems);
    }

    public async Task<CartItemDto?> GetCartItemByIdAsync(int id)
    {
        var cartItem = await cartItemRepository.GetByIdAsync(id);
        return cartItem is null ? null : mapper.Map<CartItemDto>(cartItem);
    }

    public async Task<CartItemDto?> UpdateCartItemAsync(CartItemDto cartItemDto)
    {
        var updatedCartItem = await cartItemRepository.UpdateAsync(mapper.Map<CartItem>(cartItemDto));
        return updatedCartItem is null ? null : mapper.Map<CartItemDto>(updatedCartItem);
    }

    public async Task<bool> AddToCartAsync(AddToCartDto addToCartDto, int userId)
    {
        var product = await orderUnitOfWork.ProductRepository.GetByIdAsync(addToCartDto.ProductId);
        if (product is null)
        {
            return false;
        }

        await orderUnitOfWork.WishListItemRepository.RemoveAllByConditionAsync(wli =>
            wli.UserId == userId && wli.ProductId == addToCartDto.ProductId, userId, false);

        var cartItem =
            (await orderUnitOfWork.CartItemRepository.GetByConditionAsync(c =>
                c.UserId == userId && c.ProductId == addToCartDto.ProductId)).FirstOrDefault();
        if (cartItem is null)
        {
            if (addToCartDto.Quantity <= 0)
            {
                return false;
            }

            await orderUnitOfWork.CartItemRepository.CreateAsync(new CartItem
            {
                UserId = userId,
                ProductId = addToCartDto.ProductId,
                Quantity = addToCartDto.Quantity
            }, userId, false);

            await orderUnitOfWork.Commit(userId);
            return true;
        }

        cartItem.Quantity = addToCartDto.Quantity;
        if (cartItem.Quantity <= 0)
            await orderUnitOfWork.CartItemRepository.DeleteAsync(cartItem.Id, userId, false);
        else
            await orderUnitOfWork.CartItemRepository.UpdateAsync(cartItem, userId, false);

        await orderUnitOfWork.Commit(userId);
        return true;
    }

    public Task<bool> DeleteCartItemByIdAsync(int id)
    {
        return cartItemRepository.DeleteAsync(id);
    }

    public async Task<bool> DeleteCartItemByIdAsync(int id, int userId)
    {
        var cartItem =
            (await orderUnitOfWork.CartItemRepository.GetByConditionAsync(c => c.Id == id && c.UserId == userId))
            .FirstOrDefault();
        if (cartItem is null)
        {
            return false;
        }

        if (cartItem.UserId != userId)
        {
            return false;
        }

        await orderUnitOfWork.CartItemRepository.DeleteAsync(id, userId, false);
        await orderUnitOfWork.Commit(userId);
        return true;
    }
}
