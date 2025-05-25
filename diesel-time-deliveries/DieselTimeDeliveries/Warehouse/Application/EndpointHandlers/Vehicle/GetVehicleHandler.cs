using ErrorOr;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Vehicle;

public record GetVehicleQuery(Guid VehicleId)
{
    public record Result(Guid VehicleId, string Make, string Model, decimal WeightLimit, string Vin, string Status);
}

public class GetVehicleHandler(IQueryObject<Domain.Models.Vehicle.Vehicle> queryObject)
{
    public async Task<ErrorOr<GetVehicleQuery.Result>> Handle(GetVehicleQuery query)
    {
        var vehicle = (await queryObject.Filter(p => p.Id == VehicleId.Create(query.VehicleId)).ExecuteAsync())
            .SingleOrDefault();

        if (vehicle is null)
            return Error.Validation($"Vehicle (id: {query.VehicleId}) not found");

        return new GetVehicleQuery.Result(
            vehicle.Id.Value,
            vehicle.Make,
            vehicle.Model,
            vehicle.PackageWeightLimit.Value,
            vehicle.Vin.Value,
            vehicle.Status.ToString()
        );
    }
}