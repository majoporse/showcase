using ErrorOr;
using Warehouse.Domain.Models.Package;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Package;

public record UpdatePackageCommand(Guid PackageId, string? Name, decimal? Weight, string? Destination, string? Status)
{
    public record Result(
        Guid PackageId,
        string Name,
        decimal Weight,
        string Destination,
        string Status,
        DateTime? ProcessedAt,
        DateTime? PickedForDeliveryAt,
        DateTime? DeliveredAt,
        DateTime? DiscardedAt);
}

public class updatePackageHandler(IRepository<Domain.Models.Package.Package> repository, IQueryObject<Domain.Models.Package.Package> queryObject)
{
    public async Task<ErrorOr<UpdatePackageCommand.Result>> HandleAsync(UpdatePackageCommand command)
    {
        var package = 
            (await queryObject.Filter(g => 
                g.Id == PackageId.Create(command.PackageId)).ExecuteAsync())
            .SingleOrDefault();
        
        if (package is null)
            return Error.Validation("package not found");

        
        var updateResult = package.Update(command.Name, command.Weight, command.Destination, command.Status);
        
        if (updateResult.IsError)
            return updateResult.Errors;
        
        var updateedGoods = package;
        
        updateedGoods.PackageUpdated();

        repository.Update(updateedGoods);
        await repository.CommitAsync();
        
        return new UpdatePackageCommand.Result(
            updateedGoods.Id.Value,
            updateedGoods.Name,
            updateedGoods.Weight.Value,
            updateedGoods.Destination,
            updateedGoods.Status.ToString(),
            updateedGoods.ProcessedAt,
            updateedGoods.PickedForDeliveryAt,
            updateedGoods.DeliveredAt,
            updateedGoods.DiscardedAt
        );
    }
}