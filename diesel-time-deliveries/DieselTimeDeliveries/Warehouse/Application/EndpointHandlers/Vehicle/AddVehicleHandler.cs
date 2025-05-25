using ErrorOr;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Vehicle;

public record AddVehicleCommand(string Make, string Model, decimal WeightLimit, string Vin)
{
    public record Result(Guid VehicleId, string Make, string Model, decimal WeightLimit, string Vin, string Status);
}

public class AddVehicleHandler(IRepository<Domain.Models.Vehicle.Vehicle> repository)
{
    public async Task<ErrorOr<AddVehicleCommand.Result>> HandleAsync(AddVehicleCommand command)
    {
        var goods = Domain.Models.Vehicle.Vehicle.Create(command.Make, command.Model, command.WeightLimit, command.Vin);

        if (goods.IsError)
            return goods.Errors;

        var addedGoods = await repository.InsertAsync(goods.Value);

        addedGoods.VehicleAdded();

        await repository.CommitAsync();
        return new AddVehicleCommand.Result(
            addedGoods.Id.Value,
            addedGoods.Make,
            addedGoods.Model,
            addedGoods.PackageWeightLimit.Value,
            addedGoods.Vin.Value,
            addedGoods.Status.ToString()
        );
    }
}