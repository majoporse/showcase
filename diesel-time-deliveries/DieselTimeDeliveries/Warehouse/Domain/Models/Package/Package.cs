using ErrorOr;
using SharedKernel;
using Warehouse.Domain.Events;
using Warehouse.Domain.Events.Package;
using Wolverine.Persistence;

namespace Warehouse.Domain.Models.Package;

public class Package : AggregateRoot<PackageId>
{
    public string Name { get; private set; }
    public Weight Weight { get; private set; }
    public string Destination { get; private set; }
    public PackageStatusEnum Status { get; private set; }
    
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? PickedForDeliveryAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public DateTime? DiscardedAt { get; private set; }

    public static ErrorOr<Package> Create(string name, decimal weight, string destination)
    {
        var weightOrError = Weight.Create(weight);
        if (weightOrError.IsError) return weightOrError.Errors;

        return new Package
        {
            Id = PackageId.CreateUnique(),
            Name = name,
            Weight = weightOrError.Value,
            ProcessedAt = DateTime.Now.ToUniversalTime(),
            Destination = destination,
            Status = PackageStatusEnum.Stored
        };
    }
    
    public void UpdateStatus(PackageStatusEnum status)
    {
        Status = status;
        
        switch (Status)
        {
            case PackageStatusEnum.Stored:
                ProcessedAt = DateTime.Now.ToUniversalTime();
                break;
            case PackageStatusEnum.Inbound:
                PickedForDeliveryAt = DateTime.Now.ToUniversalTime();
                break;
            case PackageStatusEnum.Delivered:
                DeliveredAt = DateTime.Now.ToUniversalTime();
                break;
            case PackageStatusEnum.Discarded:
                DiscardedAt = DateTime.Now.ToUniversalTime();
                break;
        }
    }

    public ErrorOr<Success> Update(string? name, decimal? weight, string? destination, string? status)
    {
        Name = name ?? Name;
        Destination = destination ?? Destination;
        
        if (weight.HasValue)
        {
            var weightOrError = Weight.Create(weight.Value);
            if (weightOrError.IsError) return weightOrError.Errors;
            Weight = weightOrError.Value;
        }
        
        if (status != null)
        {
            if (!Enum.TryParse<PackageStatusEnum>(status, true, out var newStatus))
                return Error.Validation("Invalid status");
            UpdateStatus(newStatus);
        }
        
        return Result.Success;
    }


    public void PackageAdded()
    {
        RaiseEvent(new PackageCreatedDomainEvent{ PackageId = Id.Value });
    }
    
    public void PackageUpdated()
    {
        RaiseEvent(new PackageUpdatedDomainEvent{ PackageId = Id.Value });
    }
    
    public void PackageDeleted()
    {
        RaiseEvent(new PackageDeletedDomainEvent{ PackageId = Id.Value });
    }
}