namespace Contracts.Queries;

public record GetAvailableVehicleQuery()
{
    public record Result(Guid VehicleId);
}