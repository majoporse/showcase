using System.Security.Claims;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Admin) + "," + nameof(UserRole.Customer))]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private const string ApiBaseName = "Order";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateOrder))]
    public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto order)
    {
        var result = await orderService.CreateOrderAsync(order);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllOrders))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
    {
        var result = await orderService.GetAllOrdersAsync();
        return Ok(result);
    }

    [HttpGet("{orderId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetOrderDetail))]
    public async Task<ActionResult<OrderDto>> GetOrderDetail(int orderId)
    {
        var result = await orderService.GetOrderDetailByIdAsync(orderId);
        if (result == null) return NotFound();
        if (User.IsInRole(UserRole.Admin.ToString())) return Ok(result);

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();
        return result.UserId != userId ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateOrder))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<OrderDto>> UpdateOrder(OrderDto order)
    {
        var result = await orderService.UpdateOrderAsync(order);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{orderId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteOrder))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeleteOrder(int orderId)
    {
        var result = await orderService.DeleteOrderByIdAsync(orderId);
        return result ? Ok() : NotFound();
    }
}