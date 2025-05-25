using Contracts.Events;
using Contracts.Queries;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Models.Package;

namespace Warehouse.Application.EventHandlers;

public class DeliveryEndedHandler(
    IRepository<Domain.Models.Vehicle.Vehicle> repositoryVehicle,
    IQueryObject<Domain.Models.Vehicle.Vehicle> queryObjectVehicle,
    IRepository<Domain.Models.Courier.Courier> repositoryCourier,
    IQueryObject<Domain.Models.Courier.Courier> queryObjectCourier
    )
{
    public async Task HandleAsync(DeliveryEndedEvent query)
    {
        var vehicle = (await queryObjectVehicle.Filter(v => v.Id == VehicleId.Create(query.VehicleId)).ExecuteAsync()).SingleOrDefault();
        var courier = (await queryObjectCourier.Filter(c => c.Id == CourierId.Create(query.CourierId)).ExecuteAsync()).SingleOrDefault();

        if (vehicle is not null)
        {
            vehicle.Status = VehicleStatusEnum.Available;
            repositoryVehicle.Update(vehicle);
        }

        if (courier is not null)
        {
            courier.Status = CourierStatusEnum.WaitingForAssignment;
            repositoryCourier.Update(courier);
        }
        
        await repositoryVehicle.CommitAsync();
    }
}