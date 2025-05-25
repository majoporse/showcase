using System.ComponentModel.DataAnnotations;
using Commons.Enums;
using JuiceWorld.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JuiceWorld.Entities;

[Index(nameof(Email), IsUnique = true)]
public class User : IdentityUser<int>, IAuditableEntity, IBaseEntity
{
    public UserRole UserRole { get; set; } = UserRole.Unknown;

    [MaxLength(300)]
    public string Bio { get; set; } = "";

    public virtual List<CartItem> CartItems { get; set; } = [];
    public virtual List<WishListItem> WishListItems { get; set; } = [];
    public virtual List<Review> Reviews { get; set; } = [];
    public virtual List<Order> Orders { get; set; } = [];
    public virtual List<AuditTrail> AuditTrails { get; set; } = [];

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}