using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Vehicle;

public class VehicleUpdatedDomainEvent : IDomainEvent
{
    public Guid VehicleId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new VehicleUpdatedIntegrationEvent
        {
            Id = VehicleId
        };
    }
}


public class VehicleUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
