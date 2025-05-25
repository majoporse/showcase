using Contracts.Commands;
using Microsoft.AspNetCore.Http;
using Routing.Application;
using Wolverine;
using ErrorOr;
using Wolverine.Http;

namespace Routing.Endpoints;

public record RouteRequest(List<string> Destinations, string origin);

public class TestEndpointRoute //only for testing purposes
{
    [Tags("Routing - Package")]
    [WolverinePost("/testroute")]
    public static async Task<IResult> RoutePackagesAsync(RouteRequest routeRequest, IMessageBus sender)
    {
        var command = new RoutePackagesCommand(routeRequest.Destinations, routeRequest.origin);

        var result = await sender.InvokeAsync<ErrorOr<RoutePackagesCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(value.Destinations),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}

public record Response(List<string> Destinations) { }