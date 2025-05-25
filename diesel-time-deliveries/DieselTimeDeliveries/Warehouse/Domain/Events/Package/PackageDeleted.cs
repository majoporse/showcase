using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Package;

public class PackageDeletedDomainEvent : IDomainEvent
{
    public Guid PackageId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new PackageDeletedIntegrationEvent
        {
            Id = PackageId
        };
    }
}


public class PackageDeletedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
