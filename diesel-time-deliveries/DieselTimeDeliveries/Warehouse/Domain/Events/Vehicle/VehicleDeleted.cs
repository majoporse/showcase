using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Vehicle;

public class VehicleDeletedDomainEvent : IDomainEvent
{
    public Guid VehicleId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new VehicleDeletedIntegrationEvent
        {
            Id = VehicleId
        };
    }
}


public class VehicleDeletedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
