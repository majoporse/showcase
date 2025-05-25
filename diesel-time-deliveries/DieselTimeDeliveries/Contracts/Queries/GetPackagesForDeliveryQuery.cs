namespace Contracts.Queries;

public record GetPackagesForDeliveryQuery(Guid VehicleId)
{
    public record Result(IEnumerable<Guid> PackageIds);
}