using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegistR.Attributes.Extensions;
using Warehouse.Infrastructure.Persistence;
using Wolverine.Attributes;

[assembly: WolverineModule]

namespace Warehouse;

public static class WarehouseInstaller
{
    public static IServiceCollection WarehouseInstall(this IServiceCollection services,
        string inventoryConnectionString)
    {
        services.InstallRegisterAttribute(Assembly.GetExecutingAssembly());
        services.AddDbContextFactory<WarehouseDbContext>(options => { options.UseNpgsql(inventoryConnectionString); });

        return services;
    }
}