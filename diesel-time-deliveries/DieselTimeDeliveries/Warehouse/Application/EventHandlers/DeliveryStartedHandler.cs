using Contracts.Events;
using Contracts.Queries;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Models.Package;

namespace Warehouse.Application.EventHandlers;

public class DeliveryStartedHandler(
    IRepository<Domain.Models.Vehicle.Vehicle> repositoryVehicle,
    IQueryObject<Domain.Models.Vehicle.Vehicle> queryObjectVehicle,
    IRepository<Domain.Models.Courier.Courier> repositoryCourier,
    IQueryObject<Domain.Models.Courier.Courier> queryObjectCourier,
    IRepository<Domain.Models.Package.Package> repositoryPackage,
    IQueryObject<Domain.Models.Package.Package> queryObjectPackage)
{
    public async Task HandleAsync(DeliveryStartedEvent query)
    {
        var vehicle = (await queryObjectVehicle.Filter(v => v.Id == VehicleId.Create(query.VehicleId)).ExecuteAsync()).SingleOrDefault();
        var courier = (await queryObjectCourier.Filter(c => c.Id == CourierId.Create(query.CourierId)).ExecuteAsync()).SingleOrDefault();
        var packages = await queryObjectPackage.Filter(p => query.PackageIds.Contains(p.Id.Value)).ExecuteAsync();

        if (vehicle is not null)
        {
            vehicle.Status = VehicleStatusEnum.InUse;
            repositoryVehicle.Update(vehicle);
        }
        
        if (courier is not null)
        {
            courier.Status = CourierStatusEnum.Delivering;
            repositoryCourier.Update(courier);
        }
        
        foreach (var package in packages)
        {
            package.UpdateStatus(PackageStatusEnum.Inbound);
            repositoryPackage.Update(package);
        }
        
        await repositoryVehicle.CommitAsync();
    }
}