using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Commons.Enums;
using JuiceWorld.Entities.Interfaces;

namespace JuiceWorld.Entities;

public class Product : BaseEntity, IAuditableEntity
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }

    public string? Image { get; set; }

    [MaxLength(1000)]
    public required string Description { get; set; }

    public ProductCategory Category { get; set; } = ProductCategory.Unknown;
    public ProductUsageType UsageType { get; set; } = ProductUsageType.Unknown;
    public int ManufacturerId { get; set; }

    [ForeignKey(nameof(ManufacturerId))]
    public virtual Manufacturer? Manufacturer { get; set; }

    public virtual List<Review> Reviews { get; set; } = [];
    public virtual List<CartItem> CartItems { get; set; } = [];
    public virtual List<WishListItem> WishListItems { get; set; } = [];
    public virtual List<OrderProduct> OrdersProducts { get; set; } = [];
    public virtual List<Tag> Tags { get; set; } = [];
}