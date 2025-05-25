using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Vehicle;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Vehicle;

public record UpdateVehicleRequest(Guid VehicleId, string? Make, string? Model, decimal? Weight, string? Vin, string? Status)
{
    public record Response(Guid VehicleId, string Make, string Model, decimal Weight, string Vin,  string Status);
}

public class UpdateVehicleEndpoint
{
    [Tags("Warehouse - Vehicle")]
    [WolverinePatch("/updateVehicle")]
    public static async Task<IResult> AddVehiclesAsync(UpdateVehicleRequest request, IMessageBus sender)
    {
        var command = new UpdateVehicleCommand(request.VehicleId, request.Make, request.Model, request.Weight, request.Vin, request.Status);

        var result = await sender.InvokeAsync<ErrorOr<UpdateVehicleCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new UpdateVehicleRequest.Response(
                    value.VehicleId,
                    value.Make,
                    value.Model,
                    value.Weight,
                    value.Vin,
                    value.Status.ToString()
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}