namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class CouponCodeViewModel
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public int GiftCardId { get; set; }
    public DateTime? RedeemedAt { get; set; }
}
