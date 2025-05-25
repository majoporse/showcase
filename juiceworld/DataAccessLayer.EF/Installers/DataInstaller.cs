using System.Diagnostics;
using Commons.Constants;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Data;
using JuiceWorld.Entities;
using JuiceWorld.QueryObjects;
using JuiceWorld.Repositories;
using JuiceWorld.UnitOfWork;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JuiceWorld.Installers;

public static class DataInstaller
{
    public static IServiceCollection DalInstall(this IServiceCollection services)
    {
        services.AddScoped<IQueryObject<User>, QueryObject<User>>();
        services.AddScoped<IQueryObject<Product>, QueryObject<Product>>();
        services.AddScoped<IQueryObject<AuditTrail>, QueryObject<AuditTrail>>();
        services.AddScoped<IQueryObject<Order>, QueryObject<Order>>();
        services.AddScoped<IQueryObject<Manufacturer>, QueryObject<Manufacturer>>();
        services.AddScoped<IQueryObject<Tag>, QueryObject<Tag>>();
        services.AddScoped<IQueryObject<CouponCode>, QueryObject<CouponCode>>();
        services.AddScoped<IQueryObject<GiftCard>, QueryObject<GiftCard>>();

        services.AddScoped<IRepository<GiftCard>, Repository<GiftCard>>();
        services.AddScoped<IRepository<CouponCode>, Repository<CouponCode>>();
        services.AddScoped<IRepository<User>, Repository<User>>();
        services.AddScoped<IRepository<CartItem>, Repository<CartItem>>();
        services.AddScoped<IRepository<Manufacturer>, Repository<Manufacturer>>();
        services.AddScoped<IRepository<Order>, Repository<Order>>();
        services.AddScoped<IRepository<OrderProduct>, Repository<OrderProduct>>();
        services.AddScoped<IRepository<Product>, Repository<Product>>();
        services.AddScoped<IRepository<Review>, Repository<Review>>();
        services.AddScoped<IRepository<WishListItem>, Repository<WishListItem>>();
        services.AddScoped<IRepository<AuditTrail>, Repository<AuditTrail>>();
        services.AddScoped<IRepository<Tag>, Repository<Tag>>();

        services.AddScoped<OrderUnitOfWork, OrderUnitOfWork>(serviceProvider =>
            new OrderUnitOfWork(serviceProvider.GetRequiredService<JuiceWorldDbContext>()));

        services.AddScoped<ProductUnitOfWork, ProductUnitOfWork>(serviceProvider =>
            new ProductUnitOfWork(serviceProvider.GetRequiredService<JuiceWorldDbContext>()));

        services.AddDbContextFactory<JuiceWorldDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable(EnvironmentConstants.DbConnectionString);

            if (connectionString == null)
                throw new Exception(
                    $"Connection string is null, make sure it is specified " +
                    $"in the environment variable: {EnvironmentConstants.DbConnectionString}");

            options
                .UseNpgsql(connectionString)
                .LogTo(s => Debug.WriteLine(s))
                .UseLazyLoadingProxies();
        });
        
        services.AddSingleton<DatabaseMigrator>();
        services.AddSingleton<IStartupFilter, MigrateDatabaseOnStartup>();

        return services;
    }
}