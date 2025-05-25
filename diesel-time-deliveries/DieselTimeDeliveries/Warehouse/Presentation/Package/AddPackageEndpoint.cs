using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Package;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Package;

public record AddPackageRequest(string Name, decimal Weight, string Destination)
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

public class AddPackageEndpoint
{
    [Tags("Warehouse - Package")]
    [WolverinePost("/addPackage")]
    public static async Task<IResult> AddPackagesAsync(AddPackageRequest request, IMessageBus sender)
    {
        var command = new AddPackageCommand(request.Name, request.Weight, request.Destination);

        var result = await sender.InvokeAsync<ErrorOr<AddPackageCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new AddPackageRequest.Response(
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