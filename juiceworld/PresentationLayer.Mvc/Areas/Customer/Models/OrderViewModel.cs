using Commons.Enums;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class OrderViewModel : BaseEntityViewModel
{
    public DeliveryType DeliveryType { get; set; } = DeliveryType.Standard;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime? Departure { get; set; }
    public DateTime? Arrival { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; } = PaymentMethodType.Monero;
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string HouseNumber { get; set; }
    public required string ZipCode { get; set; }
    public required string Country { get; set; }
    public int UserId { get; set; }
    public int AddressId { get; set; }
}
