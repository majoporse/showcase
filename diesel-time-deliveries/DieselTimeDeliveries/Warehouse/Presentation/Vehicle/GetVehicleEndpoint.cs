using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Vehicle;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Vehicle;

public record GetVehicleResponse(
    Guid VehicleId,
    string Make,
    string Model,
    decimal WeightLimit,
    string Vin,
    string Status);

public class GetVehicleEndpoint
{
    [Tags("Warehouse - Vehicle")]
    [WolverineGet("/vehicle/{id}")]
    public static async Task<IResult> GetVehicleAsync(Guid id, IMessageBus sender)
    {
        var query = new GetVehicleQuery(id);

        var result = await sender.InvokeAsync<ErrorOr<GetVehicleQuery.Result>>(query);

        return result.Match(
            value => Results.Ok(
                new GetVehicleResponse(
                    value.VehicleId,
                    value.Make,
                    value.Model,
                    value.WeightLimit,
                    value.Vin,
                    value.Status
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}