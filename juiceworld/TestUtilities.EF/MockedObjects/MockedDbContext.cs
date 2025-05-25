using JuiceWorld.Data;
using Microsoft.EntityFrameworkCore;
using TestUtilities.Data;

namespace TestUtilities.MockedObjects;

public static class MockedDbContext
{
    private static string RandomDbName => Guid.NewGuid().ToString();

    public static DbContextOptions<JuiceWorldDbContext> GetOptions()
    {
        return new DbContextOptionsBuilder<JuiceWorldDbContext>()
            .UseInMemoryDatabase(RandomDbName)
            .Options;
    }

    public static JuiceWorldDbContext CreateFromOptions(DbContextOptions<JuiceWorldDbContext> options)
    {
        var dbContext = new JuiceWorldDbContext(options);
        PrepareData(dbContext);
        return dbContext;
    }

    public static void PrepareData(JuiceWorldDbContext dbContext)
    {
        var tags = TestDataHelper.GetTestTags();
        var manufacturers = TestDataHelper.GetTestManufacturers();
        var users = TestDataHelper.GetTestUsers();
        var orders = TestDataHelper.GetTestOrders(users);
        var products = TestDataHelper.GetTestProducts(tags);
        var cartItems = TestDataHelper.GetTestCartItems(users, products);
        var orderProducts = TestDataHelper.GetTestOrderProducts(orders, products);
        var reviews = TestDataHelper.GetTestReviews(users, products);
        var wishListItems = TestDataHelper.GetTestWishListItems(users, products);

        dbContext.AddRange(tags);
        dbContext.AddRange(manufacturers);
        dbContext.AddRange(users);
        dbContext.AddRange(orders);
        dbContext.AddRange(products);
        dbContext.AddRange(cartItems);
        dbContext.AddRange(orderProducts);
        dbContext.AddRange(reviews);
        dbContext.AddRange(wishListItems);

        dbContext.SaveChanges();
    }

    public static async Task PrepareDataAsync(JuiceWorldDbContext dbContext)
    {
        var tags = TestDataHelper.GetTestTags();
        var manufacturers = TestDataHelper.GetTestManufacturers();
        var users = TestDataHelper.GetTestUsers();
        var orders = TestDataHelper.GetTestOrders(users);
        var products = TestDataHelper.GetTestProducts(tags);
        var cartItems = TestDataHelper.GetTestCartItems(users, products);
        var orderProducts = TestDataHelper.GetTestOrderProducts(orders, products);
        var reviews = TestDataHelper.GetTestReviews(users, products);
        var wishListItems = TestDataHelper.GetTestWishListItems(users, products);

        await dbContext.AddRangeAsync(tags);
        await dbContext.AddRangeAsync(manufacturers);
        await dbContext.AddRangeAsync(users);
        await dbContext.AddRangeAsync(orders);
        await dbContext.AddRangeAsync(products);
        await dbContext.AddRangeAsync(cartItems);
        await dbContext.AddRangeAsync(orderProducts);
        await dbContext.AddRangeAsync(reviews);
        await dbContext.AddRangeAsync(wishListItems);

        await dbContext.SaveChangesAsync();
    }
}