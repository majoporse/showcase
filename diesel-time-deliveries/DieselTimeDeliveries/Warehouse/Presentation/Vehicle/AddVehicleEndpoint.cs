using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Vehicle;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Vehicle;

public record AddVehicleRequest(string Make, string Model, decimal WeightLimit, string Vin)
{
    public record Response(Guid VehicleId, string Make, string Model, decimal WeightLimit, string Vin, string Status);
}

public class AddVehicleEndpoint
{
    [Tags("Warehouse - Vehicle")]
    [WolverinePost("/addVehicle")]
    public static async Task<IResult> AddVehiclesAsync(AddVehicleRequest request, IMessageBus sender)
    {
        var command = new AddVehicleCommand(request.Make, request.Model, request.WeightLimit, request.Vin);

        var result = await sender.InvokeAsync<ErrorOr<AddVehicleCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new AddVehicleRequest.Response(
                    value.VehicleId,
                    value.Make,
                    value.Model,
                    value.WeightLimit,
                    value.Vin,
                    value.Status.ToString()
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}