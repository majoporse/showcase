using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Courier;

public class CourierUpdatedDomainEvent : IDomainEvent
{
    public Guid CourierId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new CourierUpdatedIntegrationEvent
        {
            Id = CourierId
        };
    }
}


public class CourierUpdatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
