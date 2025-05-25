namespace BusinessLayer.DTOs;

public class GiftCardCreateDto
{
    public int Discount { get; set; }
    public string Name { get; set; } = "";
    public int CouponsCount { get; set; }
    public DateTime? StartDateTime { get; set; }
    public DateTime? ExpiryDateTime { get; set; }
}