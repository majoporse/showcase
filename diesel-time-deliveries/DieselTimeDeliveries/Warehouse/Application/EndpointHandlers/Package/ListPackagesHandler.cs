using ErrorOr;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Package;

public record ListPackagesQuery(int Page, int PageSize)
{
    public record Result(IEnumerable<Package> Packages);

    public record Package(
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

public class ListPackagesHandler(IQueryObject<Domain.Models.Package.Package> queryObject)
{
    public async Task<ErrorOr<ListPackagesQuery.Result>> Handle(ListPackagesQuery query)
    {
        var packages = await queryObject.Page(query.Page, query.PageSize).ExecuteAsync();

        return new ListPackagesQuery.Result(
            packages.Select(p => new ListPackagesQuery.Package(
                p.Id.Value,
                p.Name,
                p.Weight.Value,
                p.Destination,
                p.Status.ToString(),
                p.ProcessedAt,
                p.PickedForDeliveryAt,
                p.DeliveredAt,
                p.DiscardedAt
            ))
        );
    }
}