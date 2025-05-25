using Contracts.Commands;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Maps.Common;
using GoogleApi.Entities.Maps.Common.Enums;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using JasperFx.Core;
using Routing.Services;
using Wolverine.Attributes;
using ErrorOr;
using FastExpressionCompiler;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Geocoding.Location.Request;
using GoogleApi.Entities.Maps.Routes.Common;
using GoogleApi.Entities.Maps.Routes.Common.Enums;
using GoogleApi.Entities.Maps.Routes.Directions.Request;
using Error = ErrorOr.Error;

namespace Routing.Application;

public record GetDirectionsCommand(string Origin, string Destination)
{
    public record Result(List<string> StepInstructions);
}

[WolverineHandler]
public class GetDirectionsHandler(IApiKeyProvider apiKeyProvider)
{
    public async Task<ErrorOr<GetDirectionsCommand.Result?>> HandleAsync(GetDirectionsCommand command)
    {
        RoutesDirectionsRequest request = new RoutesDirectionsRequest();

        request.Key = apiKeyProvider.Key;
        request.Origin = new RouteWayPoint {Address = command.Origin };
        request.Destination = new RouteWayPoint {Address = command.Destination };
        request.TravelMode = RouteTravelMode.Drive;

        var res = await GoogleApi.GoogleMaps.Routes.RouteDirections.QueryAsync(request);
        
        if (res == null )
        {
            return new Error();
        }
        
        if (!res.Routes.Any() || !res.Routes.GetFirst().Legs.Any())
        {
            return new Error();
        }

        return new GetDirectionsCommand.Result(res.Routes.GetFirst().Legs.FirstOrDefault()?
            .Steps.Map(e => e.NavigationInstruction.Instructions).ToList() ?? []);
    }
}
