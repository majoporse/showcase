namespace BusinessLayer.DTOs;

public class CouponCodeDto
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public int GiftCardId { get; set; }
    public DateTime? RedeemedAt { get; set; }
}