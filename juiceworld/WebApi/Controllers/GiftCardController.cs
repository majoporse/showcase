using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = nameof(UserRole.Customer) + "," + nameof(UserRole.Admin))]
public class GiftCardController(IGiftCardService giftCardService) : ControllerBase
{
    private const string ApiBaseName = "giftCard";

    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [OpenApiOperation(ApiBaseName + nameof(CreategiftCard))]
    public async Task<ActionResult<GiftCardDetailDto>> CreategiftCard(GiftCardCreateDto giftCardCreate)
    {
        var result = await giftCardService.CreateGiftCardAsync(giftCardCreate);
        return result == null ? Problem() : Ok(result);
    }

    [HttpGet]
    [OpenApiOperation(ApiBaseName + nameof(GetAllgiftCards))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<GiftCardEditDto>>> GetAllgiftCards(
        [FromQuery] GiftCardFilterDto giftCardFilter)
    {
        var result = await giftCardService.GetAllGiftCardsAsync(giftCardFilter);
        return Ok(result);
    }

    [HttpGet("{giftCardId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(GetgiftCard))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<GiftCardDetailDto>> GetgiftCard(int giftCardId)
    {
        var result = await giftCardService.GetGiftCardByIdAsync(giftCardId);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPut]
    [OpenApiOperation(ApiBaseName + nameof(UpdategiftCard))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<GiftCardDetailDto>> UpdategiftCard(GiftCardEditDto giftCard)
    {
        var result = await giftCardService.UpdateGiftCardAsync(giftCard);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{giftCardId:int}")]
    [OpenApiOperation(ApiBaseName + nameof(DeletegiftCard))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<bool>> DeletegiftCard(int giftCardId)
    {
        var result = await giftCardService.DeleteGiftCardByIdAsync(giftCardId);
        return result ? Ok() : NotFound();
    }

    [HttpPost("redeem")]
    [OpenApiOperation(ApiBaseName + nameof(RedeemCouponCode))]
    [Authorize(Roles = nameof(UserRole.Customer) + "," + nameof(UserRole.Admin))]
    public async Task<ActionResult<CouponCode>> RedeemCouponCode(string couponCode)
    {
        var result = await giftCardService.RedeemCouponAsync(couponCode);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("coupon-codes")]
    [OpenApiOperation(ApiBaseName + nameof(GetCouponCodes))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<IEnumerable<CouponCodeDto>>> GetCouponCodes()
    {
        var result = await giftCardService.GetCouponCodesAsync();
        return Ok(result);
    }

    [HttpGet("coupon-codes/{couponCode}")]
    [OpenApiOperation(ApiBaseName + nameof(GetCouponCode))]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<CouponCodeDto>> GetCouponCode(string couponCode)
    {
        var result = await giftCardService.GetCouponByCodeAsync(couponCode);
        return result == null ? NotFound() : Ok(result);
    }
}