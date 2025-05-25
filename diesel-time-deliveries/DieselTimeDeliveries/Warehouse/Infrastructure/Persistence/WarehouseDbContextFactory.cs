using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Warehouse.Infrastructure.Persistence;

public class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    public WarehouseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();

        Console.WriteLine("Args: " + args.Aggregate("", (a, b) => a + Environment.NewLine + b));

        var dbConnectionString = args.FirstOrDefault();
        if (dbConnectionString == null) throw new Exception("Invalid CLI argument connection string");

        optionsBuilder.UseNpgsql(dbConnectionString);
        optionsBuilder.EnableSensitiveDataLogging();

        return new WarehouseDbContext(optionsBuilder.Options, null, null);
    }
}