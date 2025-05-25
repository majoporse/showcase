using Microsoft.AspNetCore.Http;
using Routing.Application;
using Wolverine;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace Routing.Endpoints;

public record DirectionsRequest(string origin, string destination)
{
    public record Response(List<string> Direction) { }
}

[ApiController]
public class DirectionsController(IMessageBus sender) : ControllerBase //only for testing purposes
{
    [Tags("Routing")]
    [HttpGet("/directions")]
    public async Task<IResult> RoutePackagesAsync([FromQuery] DirectionsRequest destinationRouteRequest)
    {
        var command = new GetDirectionsCommand(destinationRouteRequest.origin, destinationRouteRequest.destination);

        var result = await sender.InvokeAsync<ErrorOr<GetDirectionsCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(new DirectionsRequest.Response(value.StepInstructions)),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}

