using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Package;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Package;

public record ListPackagesResponse(IEnumerable<ListPackagesResponse.Package> Packages)
{
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

public class ListPackagesEndpoint
{
    [Tags("Warehouse - Package")]
    [WolverineGet("/packages")]
    public static async Task<IResult> ListPackagesAsync(int page, int pageSize, IMessageBus sender)
    {
        var query = new ListPackagesQuery(page, pageSize);

        var result = await sender.InvokeAsync<ErrorOr<ListPackagesQuery.Result>>(query);

        return result.Match(
            value => Results.Ok(
                new ListPackagesResponse(
                    value.Packages.Select(o =>
                        new ListPackagesResponse.Package(
                            o.PackageId,
                            o.Name,
                            o.Weight,
                            o.Destination,
                            o.Status,
                            o.ProcessedAt,
                            o.PickedForDeliveryAt,
                            o.DeliveredAt,
                            o.DiscardedAt
                        )
                    )
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}