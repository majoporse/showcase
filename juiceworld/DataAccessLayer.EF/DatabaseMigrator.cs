using JuiceWorld.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace JuiceWorld;

public class DatabaseMigrator
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseMigrator(IServiceScopeFactory scopeFactory) {
        _scopeFactory = scopeFactory;
    }

    public async Task Migrate() {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<JuiceWorldDbContext>();
        await context.Database.MigrateAsync();
    }
}