using System.ComponentModel.DataAnnotations;
using Commons.Enums;

namespace BusinessLayer.DTOs;

public class CreateOrderDto : BaseEntityDto
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
    public string? CouponCodeString { get; set; }
}
