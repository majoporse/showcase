using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Courier;

public class CourierDeletedDomainEvent : IDomainEvent
{
    public Guid CourierId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new CourierDeletedIntegrationEvent
        {
            Id = CourierId
        };
    }
}


public class CourierDeletedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
