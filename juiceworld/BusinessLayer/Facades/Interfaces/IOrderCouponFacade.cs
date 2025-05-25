using BusinessLayer.DTOs;

namespace BusinessLayer.Facades.Interfaces;

public interface IOrderCouponFacade
{
    Task<OrderDto?> CreateOrderWithCouponAsync(int userId, CreateOrderDto orderDto);
}