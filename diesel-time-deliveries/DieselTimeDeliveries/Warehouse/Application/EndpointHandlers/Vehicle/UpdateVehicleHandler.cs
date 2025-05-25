using ErrorOr;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Vehicle;

public record UpdateVehicleCommand(Guid VehicleId, string? Make, string? Model, decimal? Weight, string? Vin, string? Status)
{
    public record Result(Guid VehicleId, string Make, string Model, decimal Weight, string Vin,  string Status);
}

public class UpdateVehicleHandler(IRepository<Domain.Models.Vehicle.Vehicle> repository, IQueryObject<Domain.Models.Vehicle.Vehicle> queryObject)
{
    public async Task<ErrorOr<UpdateVehicleCommand.Result>> HandleAsync(UpdateVehicleCommand command)
    {
        var vehicle = 
            (await queryObject.Filter(g => 
                g.Id == VehicleId.Create(command.VehicleId)).ExecuteAsync())
            .SingleOrDefault();
        
        if (vehicle is null)
            return Error.Validation("vehicle not found");

        
        var updateResult = vehicle.Update(
            command.Make,
            command.Model,
            command.Weight,
            command.Vin,
            command.Status
        );
        
        if (updateResult.IsError)
            return updateResult.Errors;
        
        var updateedGoods = vehicle;

        vehicle.VehicleUpdated();

        repository.Update(updateedGoods);
        await repository.CommitAsync();
        
        return new UpdateVehicleCommand.Result(
            updateedGoods.Id.Value,
            updateedGoods.Make,
            updateedGoods.Model,
            updateedGoods.PackageWeightLimit.Value,
            updateedGoods.Vin.Value,
            updateedGoods.Status.ToString()
        );
    }
}