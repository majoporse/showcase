using System.ComponentModel.DataAnnotations;
using Commons.Enums;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class CreateOrderViewModel
{
    public DeliveryType DeliveryType { get; set; } = DeliveryType.Standard;
    public PaymentMethodType PaymentMethodType { get; set; } = PaymentMethodType.Monero;

    [Required]
    public string City { get; set; } = "";

    [Required]
    public string Street { get; set; } = "";

    [Required]
    public string HouseNumber { get; set; } = "";

    [Required]
    public string ZipCode { get; set; } = "";

    [Required]
    public string Country { get; set; } = "";

    public int UserId { get; set; }
    public int AddressId { get; set; }
    public IEnumerable<CartItemDetailViewModel> CartItems { get; set; } = [];
    public string? CouponCodeString { get; set; }
}
