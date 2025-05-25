using ErrorOr;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Package;

public record AddPackageCommand(string Name, decimal Weight, string Destination)
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

public class AddPackageHandler(IRepository<Domain.Models.Package.Package> repository)
{
    public async Task<ErrorOr<AddPackageCommand.Result>> HandleAsync(AddPackageCommand command)
    {
        var package = Domain.Models.Package.Package.Create(command.Name, command.Weight, command.Destination);

        if (package.IsError)
            return package.Errors;

        var addedPackage = await repository.InsertAsync(package.Value);
        
        addedPackage.PackageAdded();

        await repository.CommitAsync();
        return new AddPackageCommand.Result(
            addedPackage.Id.Value,
            addedPackage.Name,
            addedPackage.Weight.Value,
            addedPackage.Destination,
            addedPackage.Status.ToString(),
            addedPackage.ProcessedAt,
            addedPackage.PickedForDeliveryAt,
            addedPackage.DeliveredAt,
            addedPackage.DiscardedAt
        );
    }
}