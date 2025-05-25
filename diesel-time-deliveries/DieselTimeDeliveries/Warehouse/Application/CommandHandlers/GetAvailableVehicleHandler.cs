using Contracts.Queries;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Vehicle;

namespace Warehouse.Application.CommandHandlers;

public class GetAvailableVehicleHandler(
    IRepository<Domain.Models.Vehicle.Vehicle> repository,
    IQueryObject<Domain.Models.Vehicle.Vehicle> queryObject)
{
    public async Task<ErrorOr<GetAvailableVehicleQuery.Result>> HandleAsync(GetAvailableVehicleQuery query)
    {
        var vehicles = (await queryObject.Filter(c => c.Status == VehicleStatusEnum.Available)
                .ExecuteAsync())
            .SingleOrDefault();
        
        if (vehicles is null)
        {
            return Error.Validation("No available vehicles");
        }
        
        return new GetAvailableVehicleQuery.Result(vehicles.Id.Value);
    }
}