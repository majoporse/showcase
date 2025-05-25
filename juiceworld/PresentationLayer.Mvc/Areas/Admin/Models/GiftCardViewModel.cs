namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class GiftCardViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int Discount { get; set; }
    public int CouponsCount { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? ExpiryDateTime { get; set; }
}
