using Contracts.Events;
using Contracts.Queries;
using Warehouse.Domain.Models.Vehicle;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Models.Package;

namespace Warehouse.Application.EventHandlers;

public class PackageDeliveredHandler(
    IRepository<Domain.Models.Package.Package> repository,
    IQueryObject<Domain.Models.Package.Package> queryObject
)
{
    public async Task HandleAsync(PackageDeliveredEvent query)
    {
        
        var package = (await queryObject.Filter(p => p.Id == PackageId.Create(query.PackageId)).ExecuteAsync())
            .SingleOrDefault();

        if (package is not null)
        {
            package.UpdateStatus(query.SuccessfulDelivery ? PackageStatusEnum.Delivered : PackageStatusEnum.Discarded);
            repository.Update(package);
        }
    }
}