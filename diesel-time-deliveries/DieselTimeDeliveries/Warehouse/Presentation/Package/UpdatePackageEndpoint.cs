using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Package;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Package;

public record UpdatePackageRequest(Guid PackageId, string? Name, decimal? Weight, string? Destination, string? Status)
{
    public record Response(
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

public class UpdatePackageEndpoint
{
    [Tags("Warehouse - Package")]
    [WolverinePatch("/updatePackage")]
    public static async Task<IResult> AddPackagesAsync(UpdatePackageRequest request, IMessageBus sender)
    {
        var command = new UpdatePackageCommand(request.PackageId, request.Name, request.Weight, request.Destination, request.Status);

        var result = await sender.InvokeAsync<ErrorOr<UpdatePackageCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new UpdatePackageRequest.Response(
                    value.PackageId,
                    value.Name,
                    value.Weight,
                    value.Destination,
                    value.Status.ToString(),
                    value.ProcessedAt,
                    value.PickedForDeliveryAt,
                    value.DeliveredAt,
                    value.DiscardedAt
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}