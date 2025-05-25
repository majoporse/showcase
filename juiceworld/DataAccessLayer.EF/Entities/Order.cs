using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Commons.Enums;
using JuiceWorld.Entities.Interfaces;

namespace JuiceWorld.Entities;

public class Order : BaseEntity, IAuditableEntity
{
    public DeliveryType DeliveryType { get; set; } = DeliveryType.Unknown;
    public OrderStatus Status { get; set; } = OrderStatus.Unknown;
    public DateTime? Departure { get; set; }
    public DateTime? Arrival { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; } = PaymentMethodType.Unknown;

    [MaxLength(100)]
    public required string City { get; set; }

    [MaxLength(100)]
    public required string Street { get; set; }

    [MaxLength(10)]
    public required string HouseNumber { get; set; }

    [MaxLength(10)]
    public required string ZipCode { get; set; }

    [MaxLength(100)]
    public required string Country { get; set; }

    public int? CouponId { get; set; }

    [ForeignKey(nameof(CouponId))]
    public virtual CouponCode? CouponCode { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    public virtual List<CartItem> CartItems { get; set; } = [];
    public virtual List<OrderProduct> OrderProducts { get; set; } = [];
}