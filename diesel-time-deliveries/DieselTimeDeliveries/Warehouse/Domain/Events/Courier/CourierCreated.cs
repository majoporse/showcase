using SharedKernel.Interfaces;

namespace Warehouse.Domain.Events.Courier;

public class CourierCreatedDomainEvent : IDomainEvent
{
    public Guid CourierId { get; set; }


    public IIntegrationEvent MapToIntegrationEvent()
    {
        return new CourierCreatedIntegrationEvent
        {
            Id = CourierId
        };
    }
}


public class CourierCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; set; }
}
