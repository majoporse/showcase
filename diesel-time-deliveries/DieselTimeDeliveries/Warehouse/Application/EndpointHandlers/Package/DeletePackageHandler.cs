using ErrorOr;
using Warehouse.Domain.Models.Package;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Package;

public record DeletePackageCommand(Guid PackageId)
{
    public record Result();
}

public class DeletePackageHandler(
    IRepository<Domain.Models.Package.Package> repository,
    IQueryObject<Domain.Models.Package.Package> queryObject)
{
    public async Task<ErrorOr<DeletePackageCommand.Result>> HandleAsync(DeletePackageCommand command)
    {
        var package = (await queryObject.Filter(p => p.Id == PackageId.Create(command.PackageId)).ExecuteAsync())
            .SingleOrDefault();

        if (package is null)
        {
            return Error.NotFound("Package not found");
        }

        var packageId = PackageId.Create(command.PackageId);
        
        package.PackageDeleted();

        await repository.RemoveAsync(packageId);
        await repository.CommitAsync();

        return new DeletePackageCommand.Result();
    }
}