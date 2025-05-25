using ErrorOr;
using Warehouse.Domain.Models.Package;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Package;

public record GetPackageQuery(Guid PackageId)
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

public class GetPackageHandler(IQueryObject<Domain.Models.Package.Package> queryObject)
{
    public async Task<ErrorOr<GetPackageQuery.Result>> Handle(GetPackageQuery query)
    {
        var package = (await queryObject.Filter(p => p.Id == PackageId.Create(query.PackageId)).ExecuteAsync())
            .SingleOrDefault();

        if (package is null)
            return Error.Validation($"Package (id: {query.PackageId}) not found");

        return new GetPackageQuery.Result(
            package.Id.Value,
            package.Name,
            package.Weight.Value,
            package.Destination,
            package.Status.ToString(),
            package.ProcessedAt,
            package.PickedForDeliveryAt,
            package.DeliveredAt,
            package.DiscardedAt
        );
    }
}