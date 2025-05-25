using Contracts.Queries;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Package;
using Warehouse.Domain.Models.Vehicle;

namespace Warehouse.Application.CommandHandlers;

public class GetAvailablePackageHandler(
    IRepository<Domain.Models.Package.Package> repository,
    IQueryObject<Domain.Models.Package.Package> queryObjectPackage,
    IQueryObject<Domain.Models.Vehicle.Vehicle> queryObjectVehicle)
{
    public async Task<ErrorOr<GetPackagesForDeliveryQuery.Result>> HandleAsync(GetPackagesForDeliveryQuery query)
    {
        var vehicle = (await queryObjectVehicle.Filter(v => v.Id == VehicleId.Create(query.VehicleId)).ExecuteAsync())
            .SingleOrDefault();

        if (vehicle is null) {
            return Error.Validation("Vehicle not found");
        }

        var capacity = vehicle.PackageWeightLimit.Value;
        
        var packages = await queryObjectPackage.Filter(p => p.Status == PackageStatusEnum.Stored).ExecuteAsync();
        
        var availablePackages = new List<Guid>();
        
        var totalWeight = 0m;
        foreach (var package in packages)
        {
            if (totalWeight + package.Weight.Value <= capacity)
            {
                availablePackages.Add(package.Id.Value);
                totalWeight += package.Weight.Value;
            }
        }
        
        if (availablePackages.Count == 0)
        {
            return Error.Validation("No packages available for delivery");
        }
        
        return new GetPackagesForDeliveryQuery.Result(availablePackages);
    }
}