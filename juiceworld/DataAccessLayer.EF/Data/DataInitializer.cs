using Bogus;
using Commons.Enums;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JuiceWorld.Data;

/**
 * Class responsible for seeding the database with initial data.
 */
public static class DataInitializer
{
    private const int SeedNumber = 69420;

    private static readonly List<Tag> Tags =
    [
        new() { Id = 1, Name = "Sale", ColorHex = "#ff0000" },
        new() { Id = 2, Name = "New", ColorHex = "#00ff00" },
        new() { Id = 3, Name = "Bestseller", ColorHex = "#0000ff" },
        new() { Id = 4, Name = "Top Rated", ColorHex = "#ffff00" },
        new() { Id = 5, Name = "Recommended", ColorHex = "#00ffff" },
        new() { Id = 6, Name = "Popular", ColorHex = "#ff00ff" },
        new() { Id = 7, Name = "Trending", ColorHex = "#000000" },
        new() { Id = 8, Name = "Hot", ColorHex = "#ffffff" }
    ];

    private static readonly List<Manufacturer> Manufacturers =
    [
        new() { Id = 1, Name = "MediPharma" },
        new() { Id = 2, Name = "Royal Pharmaceuticals" },
        new() { Id = 3, Name = "Liniment Pharmaceuticals" },
        new() { Id = 4, Name = "Vermodje" },
        new() { Id = 5, Name = "Balkan Pharmaceuticals" },
        new() { Id = 6, Name = "Anfarm" },
        new() { Id = 7, Name = "Bayer" },
        new() { Id = 8, Name = "Novartis" },
        new() { Id = 9, Name = "Pfizer" },
        new() { Id = 10, Name = "Royal Pharmaceuticals" },
        new() { Id = 11, Name = "Galenika" },
        new() { Id = 12, Name = "Zambon" },
        new() { Id = 13, Name = "GlobalPharma" },
        new() { Id = 14, Name = "BM" },
        new() { Id = 15, Name = "Sport Pharmaceuticals" }
    ];

    private static readonly List<User> Users =
    [
        CreateUser(1, "user@example.com", "user", "password", "I am a steroid user!", UserRole.Customer),
        CreateUser(2, "admin@example.com", "admin", "password", "I am a steroid Admin!", UserRole.Admin)
    ];

    public static User CreateUser(int id, string email, string userName, string password, string bio, UserRole role)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            UserName = userName,
            Bio = bio,
            UserRole = role,
            NormalizedEmail = email.ToUpperInvariant(),
            NormalizedUserName = userName.ToUpperInvariant(),
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            LockoutEnabled = true
        };


        // Use PasswordHasher to hash the password
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        return user;
    }

    private static List<object> GenerateProductTags(List<Product> products, List<Tag> tags)
    {
        var productTags = new List<object>();
        foreach (var product in products)
        {
            var tag = tags[new Random().Next(3)];
            productTags.Add(new { ProductsId = product.Id, TagsId = tag.Id });
        }

        return productTags;
    }

    private static List<User> GenerateUsers()
    {
        var userIds = Users.Count + 1;
        var faker = new Faker<User>()
            .UseSeed(SeedNumber)
            .RuleFor(u => u.Id, _ => userIds++)
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Bio, f => f.Lorem.Sentence())
            .RuleFor(u => u.UserRole, f => f.PickRandom<UserRole>())
            .RuleFor(u => u.NormalizedEmail, (_, u) => u.Email?.ToUpperInvariant())
            .RuleFor(u => u.NormalizedUserName, (_, u) => u.UserName?.ToUpperInvariant())
            .RuleFor(u => u.EmailConfirmed, true)
            .RuleFor(u => u.PhoneNumberConfirmed, true)
            .RuleFor(u => u.SecurityStamp, f => Guid.NewGuid().ToString("D"))
            .RuleFor(u => u.LockoutEnabled, true);

        var users = faker.Generate(100);

        // Assign passwords using PasswordHasher
        var passwordHasher = new PasswordHasher<User>();
        foreach (var user in users) user.PasswordHash = passwordHasher.HashPassword(user, "Password123!");

        return users;
    }

    private static List<Order> GenerateOrders(List<User> users)
    {
        var orderIds = 1;
        var faker = new Faker<Order>()
            .UseSeed(SeedNumber)
            .RuleFor(o => o.Id, _ => orderIds++)
            .RuleFor(o => o.DeliveryType, f => f.PickRandom<DeliveryType>())
            .RuleFor(o => o.PaymentMethodType, f => f.PickRandom<PaymentMethodType>())
            .RuleFor(o => o.Status, f => f.PickRandom<OrderStatus>())
            .RuleFor(o => o.UserId, f => f.PickRandom(users).Id)
            .RuleFor(o => o.City, f => f.Address.City())
            .RuleFor(o => o.Street, f => f.Address.StreetName())
            .RuleFor(o => o.HouseNumber, f => f.Address.BuildingNumber())
            .RuleFor(o => o.ZipCode, f => f.Address.ZipCode())
            .RuleFor(o => o.Country, f => f.Address.Country());

        return faker.Generate(1000);
    }

    private static (List<GiftCard>, List<CouponCode>) GenerateGiftCardsWithCouponCodes()
    {
        var giftCardIds = 1;
        var faker = new Faker<GiftCard>()
            .UseSeed(SeedNumber)
            .RuleFor(gc => gc.Id, _ => giftCardIds++)
            .RuleFor(gc => gc.Discount, f => f.Random.Int(10, 1000))
            .RuleFor(gc => gc.CouponsCount, f => f.Random.Int(1, 10))
            .RuleFor(gc => gc.Name, f => f.Commerce.ProductName());

        var giftCards = faker.Generate(50);

        // for each giftcard generate appropriate ammount of coupons
        var couponCodes = new List<CouponCode>();
        var couponCodesIds = 1;
        foreach (var giftCard in giftCards)
        {
            var couponCodesFaker = new Faker<CouponCode>()
                .UseSeed(SeedNumber)
                .RuleFor(cc => cc.Id, _ => couponCodesIds++)
                .RuleFor(cc => cc.GiftCardId, giftCard.Id)
                .RuleFor(cc => cc.Code, f => Guid.NewGuid().ToString())
                .RuleFor(cc => cc.RedeemedAt, f => f.Random.Bool(0.5f) ? f.Date.Past().ToUniversalTime() : null);

            couponCodes.AddRange(couponCodesFaker.Generate(giftCard.CouponsCount));
        }

        return (giftCards, couponCodes);
    }

    private static List<CartItem> GenerateCartItems(List<User> users)
    {
        var cartItemIds = 1;
        var faker = new Faker<CartItem>()
            .UseSeed(SeedNumber)
            .RuleFor(c => c.Id, _ => cartItemIds++)
            .RuleFor(c => c.ProductId, f => f.Random.Int(1, 47))
            .RuleFor(c => c.UserId, f => f.PickRandom(users).Id)
            .RuleFor(c => c.Quantity, f => f.Random.Int(1, 10));

        return faker.Generate(500);
    }

    private static List<OrderProduct> GenerateOrderProducts(List<Order> orders)
    {
        var orderProductIds = 1;
        var faker = new Faker<OrderProduct>()
            .UseSeed(SeedNumber)
            .RuleFor(op => op.Id, _ => orderProductIds++)
            .RuleFor(op => op.OrderId, f => f.PickRandom(orders).Id)
            .RuleFor(op => op.ProductId, f => f.Random.Int(1, 47))
            .RuleFor(op => op.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(op => op.Price, f => f.Random.Decimal(1, 1000));

        return faker.Generate(2000);
    }

    private static List<Review> GenerateReviews(List<User> users)
    {
        var reviewIds = 1;
        var faker = new Faker<Review>()
            .UseSeed(SeedNumber)
            .RuleFor(r => r.Id, _ => reviewIds++)
            .RuleFor(r => r.ProductId, f => f.Random.Int(1, 47))
            .RuleFor(r => r.UserId, f => f.PickRandom(users).Id)
            .RuleFor(r => r.Rating, f => f.Random.Int(1, 5))
            .RuleFor(r => r.Body, f => f.Lorem.Sentence());

        return faker.Generate(2000);
    }

    private static List<WishListItem> GenerateWishListItems(List<User> users)
    {
        var wishListItemIds = 1;
        var faker = new Faker<WishListItem>()
            .UseSeed(SeedNumber)
            .RuleFor(w => w.Id, _ => wishListItemIds++)
            .RuleFor(w => w.ProductId, f => f.Random.Int(1, 47))
            .RuleFor(w => w.UserId, f => f.PickRandom(users).Id);

        return faker.Generate(300);
    }

    public static void Seed(this ModelBuilder modelBuilder)
    {
        var users = GenerateUsers();
        var orders = GenerateOrders(users);
        var cartItems = GenerateCartItems(users);
        var orderProducts = GenerateOrderProducts(orders);
        var reviews = GenerateReviews(users);
        var wishListItems = GenerateWishListItems(users);
        var productTags = GenerateProductTags(ProductsSeedData.Products, Tags);
        var (giftCards, couponCodes) = GenerateGiftCardsWithCouponCodes();

        // Not generated (authentic) tags, manufacturer and product data
        modelBuilder.Entity<Tag>().HasData(Tags);
        modelBuilder.Entity<Manufacturer>().HasData(Manufacturers);
        modelBuilder.Entity<Product>().HasData(ProductsSeedData.Products);

        modelBuilder.Entity<Product>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Products)
            .UsingEntity(j => j.HasData(productTags));

        // Fixed users for testing
        modelBuilder.Entity<User>().HasData(Users);

        // Generated data
        modelBuilder.Entity<User>().HasData(users);
        modelBuilder.Entity<Order>().HasData(orders);
        modelBuilder.Entity<CartItem>().HasData(cartItems);
        modelBuilder.Entity<OrderProduct>().HasData(orderProducts);
        modelBuilder.Entity<Review>().HasData(reviews);
        modelBuilder.Entity<WishListItem>().HasData(wishListItems);
        modelBuilder.Entity<GiftCard>().HasData(giftCards);
        modelBuilder.Entity<CouponCode>().HasData(couponCodes);
    }
}