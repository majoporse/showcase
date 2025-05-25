using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Package;

public class PackageCreatedDomainEvent : IDomainEvent
{
    public Guid PackageId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new PackageCreatedIntegrationEvent
        {
            Id = PackageId
        };
    }
}


public class PackageCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
