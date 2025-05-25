using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;

namespace BusinessLayer.Services;

public class GiftCardService(
    IRepository<GiftCard> giftCardRepository,
    IRepository<CouponCode> couponCodeRepository,
    IQueryObject<CouponCode> couponCodeQueryObject,
    IQueryObject<GiftCard> giftCardQueryObject,
    IMapper mapper) : IGiftCardService
{
    public async Task<GiftCardDetailDto?> CreateGiftCardAsync(GiftCardCreateDto giftCardCreateDto)
    {
        giftCardCreateDto.ExpiryDateTime = giftCardCreateDto.ExpiryDateTime?.ToUniversalTime();
        giftCardCreateDto.StartDateTime = giftCardCreateDto.StartDateTime?.ToUniversalTime();
        var newGiftCard = await giftCardRepository.CreateAsync(mapper.Map<GiftCard>(giftCardCreateDto));

        if (newGiftCard is null)
        {
            return null;
        }

        foreach (var coupon in Enumerable.Range(0, giftCardCreateDto.CouponsCount))
            await couponCodeRepository.CreateAsync(new CouponCode
            {
                Code = Guid.NewGuid().ToString(),
                GiftCardId = newGiftCard.Id
            });

        var newGiftCardAfterInsert = await giftCardRepository.GetByIdAsync(newGiftCard.Id);
        return mapper.Map<GiftCardDetailDto?>(newGiftCardAfterInsert);
    }

    public async Task<FilteredResult<GiftCardEditDto>> GetAllGiftCardsAsync(GiftCardFilterDto giftCardFilterDto)
    {
        var filteredGiftCards = await giftCardQueryObject
            .Filter(g => giftCardFilterDto.Name == null ||
                         g.Name.ToLower().Contains(giftCardFilterDto.Name.ToLower()))
            .OrderBy(p => p.Id)
            .Paginate(giftCardFilterDto.PageIndex, giftCardFilterDto.PageSize)
            .ExecuteAsync();

        return new FilteredResult<GiftCardEditDto>
        {
            Entities = mapper.Map<List<GiftCardEditDto>>(filteredGiftCards.Entities),
            PageIndex = filteredGiftCards.PageIndex,
            TotalPages = filteredGiftCards.TotalPages
        };
    }

    public async Task<GiftCardDetailDto?> GetGiftCardByIdAsync(int id)
    {
        var giftCard = await giftCardRepository.GetByIdAsync(id);
        return giftCard is null ? null : mapper.Map<GiftCardDetailDto>(giftCard);
    }

    public async Task<GiftCardDetailDto?> UpdateGiftCardAsync(GiftCardEditDto giftCardDto)
    {
        var updatedGiftCard = await giftCardRepository.UpdateAsync(mapper.Map<GiftCard>(giftCardDto));
        return updatedGiftCard is null ? null : mapper.Map<GiftCardDetailDto>(updatedGiftCard);
    }

    public Task<bool> DeleteGiftCardByIdAsync(int id)
    {
        return giftCardRepository.DeleteAsync(id);
    }

    public async Task<CouponCodeDto?> RedeemCouponAsync(string couponCode)
    {
        var redeemedCoupon = (await couponCodeQueryObject.Filter(c => c.Code == couponCode)
            .ExecuteAsync()).Entities.FirstOrDefault();

        if (redeemedCoupon == null)
            return null;

        if (redeemedCoupon.GiftCard?.ExpiryDateTime < DateTime.UtcNow)
            return null;

        if (redeemedCoupon.RedeemedAt != null)
            return null;

        redeemedCoupon.RedeemedAt = DateTime.UtcNow;
        var ret = await couponCodeRepository.UpdateAsync(redeemedCoupon);
        return ret is null ? null : mapper.Map<CouponCodeDto>(ret);
    }

    public Task<List<CouponCodeDto>> GetCouponCodesAsync()
    {
        return couponCodeRepository.GetAllAsync()
            .ContinueWith(task => mapper.Map<List<CouponCodeDto>>(task.Result));
    }

    public async Task<CouponCodeDto?> GetCouponCodeByIdAsync(int id)
    {
        var coupon = await couponCodeRepository.GetByIdAsync(id);
        return coupon is null ? null : mapper.Map<CouponCodeDto>(coupon);
    }

    public async Task<CouponCodeDto?> GetCouponByCodeAsync(string code)
    {
        var coupon = (await couponCodeQueryObject.Filter(c => c.Code == code).ExecuteAsync()).Entities.FirstOrDefault();
        return coupon is null ? null : mapper.Map<CouponCodeDto>(coupon);
    }
}
