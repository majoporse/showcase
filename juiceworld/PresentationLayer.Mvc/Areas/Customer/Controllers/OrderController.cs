using System.Security.Claims;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Facades.Interfaces;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Customer.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Controllers;

[Area(Constants.Areas.Customer)]
[RedirectIfNotAuthenticatedActionFilter]
public class OrderController(
    IOrderService orderService,
    ICartItemService cartItemService,
    IOrderCouponFacade orderCouponFacade,
    IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<ActionResult> Index([FromQuery] PaginationViewModel pagination)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var orders = await orderService.GetOrdersByUserIdAsync(userId, mapper.Map<PaginationDto>(pagination));
        return View(mapper.Map<FilteredResult<OrderViewModel>>(orders));
    }

    [HttpGet]
    public async Task<ActionResult> Details(int id)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var order = await orderService.GetOrderDetailByIdAsync(id);
        if (order is null || order.UserId != userId)
        {
            return Unauthorized();
        }

        return View(mapper.Map<OrderDetailViewModel>(order));
    }

    [HttpGet]
    public async Task<ActionResult> Create()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var cartItems = await cartItemService.GetCartItemsByUserIdAsync(userId);

        return View(new CreateOrderViewModel { UserId = userId, CartItems = mapper.Map<IEnumerable<CartItemDetailViewModel>>(cartItems) });
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateOrderViewModel orderViewModel)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }
        var cartItems = mapper.Map<IEnumerable<CartItemDetailViewModel>>(await cartItemService.GetCartItemsByUserIdAsync(userId));

        if (!ModelState.IsValid)
        {
            orderViewModel.CartItems = cartItems;
            return View(orderViewModel);
        }

        if (orderViewModel.UserId != userId)
        {
            return View(Constants.Views.BadRequest);
        }

        var order = await orderCouponFacade.CreateOrderWithCouponAsync(userId, mapper.Map<CreateOrderDto>(orderViewModel));
        if (order is null)
        {
            ModelState.AddModelError("Order", "Failed to create order.");
            orderViewModel.CartItems = cartItems;
            return View(orderViewModel);
        }

        return RedirectToAction(nameof(Details), new { order.Id });
    }
}
