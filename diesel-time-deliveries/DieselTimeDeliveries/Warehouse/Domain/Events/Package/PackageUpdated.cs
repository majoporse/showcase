using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Package;

public class PackageUpdatedDomainEvent : IDomainEvent
{
    public Guid PackageId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new PackageUpdatedIntegrationEvent
        {
            Id = PackageId
        };
    }
}


public class PackageUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
