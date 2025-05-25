using ErrorOr;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Vehicle;

public record DeleteVehicleCommand(Guid VehicleId)
{
    public record Result();
}

public class DeleteVehicleHandler(
    IRepository<Domain.Models.Vehicle.Vehicle> repository,
    IQueryObject<Domain.Models.Vehicle.Vehicle> queryObject)
{
    public async Task<ErrorOr<DeleteVehicleCommand.Result>> HandleAsync(DeleteVehicleCommand command)
    {
        var vehicle = (await queryObject.Filter(p => p.Id == VehicleId.Create(command.VehicleId)).ExecuteAsync())
            .SingleOrDefault();

        if (vehicle is null)
        {
            return Error.NotFound("Vehicle not found");
        }

        var vehicleId = VehicleId.Create(command.VehicleId);

        vehicle.VehicleDeleted();
        
        await repository.RemoveAsync(vehicleId);
        await repository.CommitAsync();

        return new DeleteVehicleCommand.Result();
    }
}