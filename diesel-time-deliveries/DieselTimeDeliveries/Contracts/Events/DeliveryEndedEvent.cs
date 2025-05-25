namespace Contracts.Events;

public record DeliveryEndedEvent(Guid VehicleId, Guid CourierId);