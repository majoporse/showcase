using System.Linq.Expressions;
using Commons.Enums;
using JuiceWorld.Entities;
using JuiceWorld.Entities.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace JuiceWorld.Data;

/**
 * The database context for the JuiceWorld database.
 */
public class JuiceWorldDbContext(DbContextOptions<JuiceWorldDbContext> options)
    : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public override DbSet<User> Users { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<WishListItem> WishListItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<CouponCode> CouponCodes { get; set; }
    public DbSet<GiftCard> GiftCards { get; set; }

    private void SetAuditableProperties()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
    }

    private List<AuditTrail> GenerateAuditTrails(int userId, CancellationToken cancellationToken = default)
    {
        var auditableEntries = ChangeTracker.Entries<IAuditableEntity>()
            .Where(x => x.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .Select(x => CreateAuditTrail(userId, x))
            .ToList();

        return auditableEntries;
    }

    private static AuditTrail CreateAuditTrail(int userId, EntityEntry<IAuditableEntity> entry)
    {
        List<string> properties = [];
        var auditTrail = new AuditTrail
        {
            EntityName = entry.Entity.GetType().Name,
            UserId = userId,
            TrailType = entry.State switch
            {
                EntityState.Added => TrailType.Create,
                EntityState.Modified => TrailType.Update,
                EntityState.Deleted => TrailType.Delete,
                _ => TrailType.None
            }
        };

        foreach (var property in entry.Properties.Where(prop => !prop.IsTemporary))
        {
            if (property.Metadata.IsPrimaryKey()) auditTrail.PrimaryKey = (int)(property.OriginalValue ?? -1);

            if (property.IsModified) properties.Add(property.Metadata.Name);
        }

        auditTrail.ChangedColumns = properties;
        return auditTrail;
    }

    public override int SaveChanges()
    {
        SetAuditableProperties();
        return base.SaveChanges();
    }

    public int SaveChanges(int userId)
    {
        AddRange(GenerateAuditTrails(userId));
        return SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditableProperties();
        return base.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(int userId, CancellationToken cancellationToken = default)
    {
        await AddRangeAsync(GenerateAuditTrails(userId, cancellationToken), cancellationToken);
        return await SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Category)
            .HasConversion<string>();

        modelBuilder.Entity<Product>()
            .Property(p => p.UsageType)
            .HasConversion<string>();

        // Product -> Manufacturer
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Manufacturer)
            .WithMany(manufacturer => manufacturer.Products)
            .HasForeignKey(p => p.ManufacturerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Product -> Tag
        modelBuilder.Entity<Product>()
            .HasMany(p => p.Tags)
            .WithMany(tag => tag.Products);

        // CartItem -> Product
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Product)
            .WithMany(product => product.CartItems)
            .HasForeignKey(ci => ci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // CartItem -> User
        modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.User)
            .WithMany(user => user.CartItems)
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review -> Product
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(product => product.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review -> User
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(user => user.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // WishListItem -> Product
        modelBuilder.Entity<WishListItem>()
            .HasOne(wl => wl.Product)
            .WithMany(product => product.WishListItems)
            .HasForeignKey(wl => wl.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // WishListItem -> User
        modelBuilder.Entity<WishListItem>()
            .HasOne(wl => wl.User)
            .WithMany(user => user.WishListItems)
            .HasForeignKey(wl => wl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Order -> User
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .Property(o => o.DeliveryType)
            .HasConversion<string>();

        modelBuilder.Entity<Order>()
            .Property(o => o.PaymentMethodType)
            .HasConversion<string>();

        // OrderProduct -> Product
        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(product => product.OrdersProducts)
            .HasForeignKey(op => op.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderProduct -> Order
        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(order => order.OrderProducts)
            .HasForeignKey(op => op.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .Property(u => u.UserRole)
            .HasConversion<string>();

        // AuditTrail -> User
        modelBuilder.Entity<AuditTrail>()
            .HasOne(at => at.User)
            .WithMany(user => user.AuditTrails)
            .HasForeignKey(at => at.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AuditTrail>()
            .Property(at => at.TrailType)
            .HasConversion<string>();

        modelBuilder.Entity<CouponCode>()
            .HasOne(cc => cc.GiftCard)
            .WithMany(gc => gc.CouponCodes)
            .HasForeignKey(cc => cc.GiftCardId)
            .OnDelete(DeleteBehavior.Cascade);

        // Global query filter for soft-deleted entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType)) continue;

            var parameter = Expression.Parameter(entityType.ClrType);
            var filter = Expression.Lambda(
                Expression.Equal(
                    Expression.Property(parameter, nameof(BaseEntity.DeletedAt)),
                    Expression.Constant(null)
                ),
                parameter
            );
            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
        }

        base.OnModelCreating(modelBuilder);

        modelBuilder.Seed();
    }
}
