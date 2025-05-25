namespace Contracts.Events;

public record DeliveryStartedEvent(Guid VehicleId, Guid CourierId, List<Guid> PackageIds);