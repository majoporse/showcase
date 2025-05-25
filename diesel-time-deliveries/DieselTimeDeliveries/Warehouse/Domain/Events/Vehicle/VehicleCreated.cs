using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Vehicle;

public class VehicleCreatedDomainEvent : IDomainEvent
{
    public Guid VehicleId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new VehicleCreatedIntegrationEvent
        {
            Id = VehicleId
        };
    }
}


public class VehicleCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
