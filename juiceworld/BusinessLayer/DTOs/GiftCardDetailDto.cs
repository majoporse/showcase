namespace BusinessLayer.DTOs;

public class GiftCardDetailDto : GiftCardViewDto
{
    public List<CouponCodeDto> CouponCodes { get; set; } = [];
}