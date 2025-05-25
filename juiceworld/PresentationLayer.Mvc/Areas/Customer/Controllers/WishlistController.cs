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
public class WishlistController(IWishListItemService wishListItemService, ICartItemService cartItemService, IMapper mapper) : Controller
{
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var wishListItems = await wishListItemService.GetWishListItemsByUserIdAsync(userId);
        return View(mapper.Map<IEnumerable<WishlistItemDetailViewModel>>(wishListItems));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var success = await wishListItemService.DeleteWishListItemByIdAsync(id);
        if (!success)
        {
            ViewData[Constants.Keys.ErrorMessage] = "Failed to delete item from wishlist.";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddToCart(int productId)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? string.Empty, out var userId))
        {
            return View(Constants.Views.BadRequest);
        }

        var addToCartDto = new AddToCartDto
        {
            ProductId = productId,
            Quantity = 1
        };

        var success = await cartItemService.AddToCartAsync(addToCartDto, userId);
        if (!success)
        {
            ViewData[Constants.Keys.ErrorMessage] = "Failed to add item to cart.";
        }

        return RedirectToAction(nameof(Index));
    }
}
