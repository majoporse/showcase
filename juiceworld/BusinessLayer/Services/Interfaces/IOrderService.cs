using BusinessLayer.DTOs;
using Infrastructure.QueryObjects;

namespace BusinessLayer.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> ExecuteOrderAsync(CreateOrderDto orderDto, int? couponId);
    Task<OrderDto?> CreateOrderAsync(CreateOrderDto orderDto);
    Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
    Task<FilteredResult<OrderDto>> GetOrdersAsync(PaginationDto orderFilter);

    Task<FilteredResult<OrderDto>> GetOrdersByUserIdAsync(int userId, PaginationDto paginationDto);
    Task<OrderDto?> GetOrderByIdAsync(int id);
    Task<OrderDetailDto?> GetOrderDetailByIdAsync(int id);
    Task<OrderDto?> UpdateOrderAsync(OrderDto orderDto);
    Task<OrderDto?> UpdateOrderAsync(OrderDetailDto orderDto);
    Task<bool> DeleteOrderByIdAsync(int id);
}
