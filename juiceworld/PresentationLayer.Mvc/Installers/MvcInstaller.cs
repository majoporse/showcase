using System.Diagnostics;
using BusinessLayer.Facades;
using BusinessLayer.Facades.Interfaces;
using Commons.Constants;
using JuiceWorld.Data;
using JuiceWorld.Entities;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;

namespace PresentationLayer.Mvc.Installers;

public static class MvcInstaller
{
    public static IServiceCollection MvcInstall(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddAutoMapper(typeof(MvcMapperInstaller));

        services.AddScoped<ISearchablesFacade, SearchablesFacade>();
        services.AddScoped<IOrderCouponFacade, OrderCouponFacade>();

        services.AddIdentity<User, IdentityRole<int>>()
            .AddEntityFrameworkStores<JuiceWorldDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options => { options.LoginPath = "/Account/Login"; });

        // Configure Logging
        var connectionString = Environment.GetEnvironmentVariable(EnvironmentConstants.LoggingDbConnectionString);
        var collectionName = Environment.GetEnvironmentVariable(EnvironmentConstants.LoggingDbCollectionName);
        var databaseName = Environment.GetEnvironmentVariable(EnvironmentConstants.LoggingDbDatabaseName);

        if (connectionString is null || collectionName is null || databaseName is null)
        {
            Debug.Fail(
                $"Logging database connection string, collection name or database name is null, make sure they are specified " +
                $"in the environment variables: " +
                $"{EnvironmentConstants.LoggingDbConnectionString} ({connectionString}), " +
                $"{EnvironmentConstants.LoggingDbCollectionName} ({collectionName}), " +
                $"{EnvironmentConstants.LoggingDbDatabaseName} ({databaseName})");
        }
        else
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.MongoDBCapped(database, collectionName: collectionName)
                .WriteTo.Console(LogEventLevel.Information)
                .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog();
            });
        }

        return services;
    }
}