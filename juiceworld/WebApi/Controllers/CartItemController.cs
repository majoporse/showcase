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
public class CartItemController(ICartItemService cartItemService) : ControllerBase
{
    private const string ApiBaseName = "CartItem";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateCartItem))]
    public async Task<ActionResult<CartItemDto>> CreateCartItem(CartItemDto cartItem)
    {
        if (cartItem.Quantity < 1) return BadRequest("Quantity must be greater than 0");
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();
        if (!User.IsInRole(UserRole.Admin.ToString()) &&
            cartItem.UserId != userId)
            return Unauthorized();

        var result = await cartItemService.CreateCartItemAsync(cartItem);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllCartItems))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<CartItemDto>>> GetAllCartItems()
    {
        var result = await cartItemService.GetAllCartItemsAsync();
        return Ok(result);
    }

    [HttpGet("{cartItemId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetCartItem))]
    public async Task<ActionResult<CartItemDto>> GetCartItem(int cartItemId)
    {
        if (await IsAuthorized(cartItemId) != Ok()) return Unauthorized();

        var result = await cartItemService.GetCartItemByIdAsync(cartItemId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateCartItem))]
    public async Task<ActionResult<CartItemDto>> UpdateCartItem(CartItemDto cartItem)
    {
        if (cartItem.Quantity < 1) return BadRequest("Quantity must be greater than 0");
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();

        if (!User.IsInRole(UserRole.Admin.ToString()) &&
            cartItem.UserId != userId)
            return Unauthorized();

        var result = await cartItemService.UpdateCartItemAsync(cartItem);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{cartItemId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteCartItem))]
    public async Task<ActionResult<bool>> DeleteCartItem(int cartItemId)
    {
        if (await IsAuthorized(cartItemId) != Ok()) return Unauthorized();
        var result = await cartItemService.DeleteCartItemByIdAsync(cartItemId);
        return result ? Ok() : NotFound();
    }

    private async Task<IActionResult> IsAuthorized(int cartItemId)
    {
        var cartItem = await cartItemService.GetCartItemByIdAsync(cartItemId);
        if (cartItem == null) return NotFound();

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var userId)) return Unauthorized();

        if (!User.IsInRole(UserRole.Admin.ToString()) &&
            cartItem.UserId != userId)
            return Unauthorized();

        return Ok();
    }
}