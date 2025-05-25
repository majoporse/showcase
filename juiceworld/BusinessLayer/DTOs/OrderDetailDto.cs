using Commons.Enums;
using JuiceWorld.Entities;

namespace BusinessLayer.DTOs;

public class OrderDetailDto
{
    public int Id { get; set; }
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
    public UserDto? User { get; set; }
    public int UserId { get; set; }

    public CouponCode? CouponCode { get; set; }
    public List<OrderProductDetailDto> OrderProducts { get; set; } = [];
}
