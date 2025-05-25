using BusinessLayer.DTOs;
using BusinessLayer.Facades.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Facades;

public class OrderCouponFacade(
    IGiftCardService giftCardService,
    IOrderService orderService,
    IUserService userService) : IOrderCouponFacade
{
    public async Task<OrderDto?> CreateOrderWithCouponAsync(int userId, CreateOrderDto orderDto)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null) return null;

        if (orderDto.CouponCodeString != null &&
            await giftCardService.GetCouponByCodeAsync(orderDto.CouponCodeString) == null) return null;

        var cc = orderDto.CouponCodeString != null
            ? await giftCardService.RedeemCouponAsync(orderDto.CouponCodeString)
            : null;

        var order = await orderService.ExecuteOrderAsync(orderDto, cc?.Id);
        if (order == null) return null;

        return order;
    }
}
