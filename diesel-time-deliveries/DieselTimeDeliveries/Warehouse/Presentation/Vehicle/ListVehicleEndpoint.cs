using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Vehicle;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Vehicle;

public record ListVehiclesResponse(IEnumerable<ListVehiclesResponse.Vehicle> Vehicles)
{
    public record Vehicle(Guid Id, string Make, string Model, decimal WeightLimit, string Vin, string Status);
}

public class ListVehicleEndpoint
{
    [Tags("Warehouse - Vehicle")]
    [WolverineGet("/vehicles")]
    public static async Task<IResult> ListVehiclesAsync(int page, int pageSize, IMessageBus sender)
    {
        var query = new ListVehiclesQuery(page, pageSize);

        var result = await sender.InvokeAsync<ErrorOr<ListVehiclesQuery.Result>>(query);

        return result.Match(
            value => Results.Ok(
                new ListVehiclesResponse(
                    value.Vehicles.Select(o =>
                        new ListVehiclesResponse.Vehicle(
                            o.VehicleId,
                            o.Make,
                            o.Model,
                            o.WeightLimit,
                            o.Vin,
                            o.Status)
                    )
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}