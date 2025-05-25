using System.Reflection;
using JasperFx.Core;
using Marten;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Models.Package;
using Warehouse.Domain.Models.Vehicle;
using Wolverine;

namespace Warehouse.Infrastructure.Persistence;

public class WarehouseDbContext : DbContext
{
    private readonly IMessageBus _sender;
    private readonly IDocumentStore _store;

    // public WarehouseDbContext() {    } ?????????????????

    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options, IMessageBus sender, IDocumentStore store) :
        base(options)
    {
        Console.WriteLine("Message bus: " + (sender == null ? "null" : sender));
        _sender = sender;
        _store = store;
    }

    public DbSet<Package> Packages { get; set; } = null!;
    public DbSet<Courier> Couriers { get; set; } = null!;
    public DbSet<Vehicle> Vehicles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Seed();
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishEventsAsync();

        return result;
    }

    private async Task PublishEventsAsync()
    {
        var domainEvents = ChangeTracker.Entries<BaseEntity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e =>
            {
                var events = e.DomainEvents;

                e.ClearEvents();

                return events;
            }).ToList();

        var integrationEvents = domainEvents
            .Map(e => e.MapToIntegrationEvent())
            .Select(e => e!)
            .ToList();

        foreach (var integrationEvent in integrationEvents) await _sender.PublishAsync(integrationEvent);
        await _store.BulkInsertDocumentsAsync(integrationEvents);
    }
}