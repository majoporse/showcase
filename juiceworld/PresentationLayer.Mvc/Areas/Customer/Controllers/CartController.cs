using System.Security.Claims;
using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.Mvc.ActionFilters;
using PresentationLayer.Mvc.Areas.Customer.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Controllers;

[Area(Constants.Areas.Customer)]
[RedirectIfNotAuthenticatedActionFilter]
public class CartController(ICartItemService cartItemService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var cartItems = await cartItemService.GetCartItemsByUserIdAsync(userId);
        return View(cartItems);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartViewModel addToCartViewModel)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var success = await cartItemService.AddToCartAsync(mapper.Map<AddToCartDto>(addToCartViewModel), userId);
        if (!success) ViewData[Constants.Keys.ErrorMessage] = "Failed to add item to cart.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteCartItem(int id)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var success = await cartItemService.DeleteCartItemByIdAsync(id, userId);
        if (!success) ViewData[Constants.Keys.ErrorMessage] = "Failed to delete item from cart.";

        return RedirectToAction(nameof(Index));
    }
}
