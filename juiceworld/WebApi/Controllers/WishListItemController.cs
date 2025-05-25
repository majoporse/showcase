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
public class WishListItemController(IWishListItemService wishListItemService) : ControllerBase
{
    private const string ApiBaseName = "WishListItem";

    [HttpPost]
    [OpenApiOperation(ApiBaseName + nameof(CreateWishListItem))]
    public async Task<ActionResult<WishListItemDto>> CreateWishListItem(WishListItemDto wishListItem)
    {
        var result = await wishListItemService.CreateWishListItemAsync(wishListItem);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllWishListItems))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<WishListItemDto>>> GetAllWishListItems()
    {
        var result = await wishListItemService.GetAllWishListItemsAsync();
        return Ok(result);
    }

    [HttpGet("{wishListItemId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetWishListItem))]
    public async Task<ActionResult<WishListItemDto>> GetWishListItem(int wishListItemId)
    {
        if (!await IsUserAuthorized(wishListItemId))
            return Unauthorized();
        var result = await wishListItemService.GetWishListItemByIdAsync(wishListItemId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdateWishListItem))]
    public async Task<ActionResult<WishListItemDto>> UpdateWishListItem(WishListItemDto wishListItem)
    {
        if (!await IsUserAuthorized(wishListItem.Id))
            return Unauthorized();
        var result = await wishListItemService.UpdateWishListItemAsync(wishListItem);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{wishListItemId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeleteWishListItem))]
    public async Task<ActionResult<bool>> DeleteWishListItem(int wishListItemId)
    {
        if (!await IsUserAuthorized(wishListItemId))
            return Unauthorized();

        var result = await wishListItemService.DeleteWishListItemByIdAsync(wishListItemId);
        return result ? Ok() : NotFound();
    }

    private async Task<bool> IsUserAuthorized(int wishListItemId)
    {
        var wishListItem = await wishListItemService.GetWishListItemByIdAsync(wishListItemId);
        if (wishListItem == null)
            return false;

        return IsUserIdAuthorized(wishListItem.UserId);
    }

    private bool IsUserIdAuthorized(int userId)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.Sid) ?? "", out var currentUserId)) return false;

        if (!User.IsInRole(UserRole.Admin.ToString()) &&
            currentUserId != userId)
            return false;
        return true;
    }
}