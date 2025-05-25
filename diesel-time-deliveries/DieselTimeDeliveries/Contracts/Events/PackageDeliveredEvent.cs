namespace Contracts.Events;

public record PackageDeliveredEvent(Guid PackageId, bool SuccessfulDelivery = true);
