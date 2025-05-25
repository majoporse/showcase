using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Customer.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Controllers;

[Area(Constants.Areas.Admin)]
[RedirectIfNotAdminActionFilter]
public class OrderController(IOrderService orderService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PaginationViewModel pagination)
    {
        var orders = await orderService.GetOrdersAsync(mapper.Map<PaginationDto>(pagination));
        return View(mapper.Map<FilteredResult<OrderViewModel>>(orders));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var order = mapper.Map<OrderDetailViewModel>(await orderService.GetOrderDetailByIdAsync(id));
        if (order == null)
        {
            ModelState.AddModelError("Id", "Order not found.");
            return View(order);
        }

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(OrderDetailViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var updatedOrder = await orderService.UpdateOrderAsync(mapper.Map<OrderDetailDto>(viewModel));
        if (updatedOrder == null)
        {
            ModelState.AddModelError(nameof(OrderDetailDto.Status), "Failed to update order.");
            return View(viewModel);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (!await orderService.DeleteOrderByIdAsync(id))
        {
            ModelState.AddModelError("Id", "Failed to delete order.");
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
}
