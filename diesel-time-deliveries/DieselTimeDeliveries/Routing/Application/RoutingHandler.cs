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


[WolverineHandler]
public class RoutingHandler(IApiKeyProvider _apiKeyProvider)
{
    public async Task<ErrorOr<RoutePackagesCommand.Result?>> HandleAsync(RoutePackagesCommand command)
    {
        RoutesDirectionsRequest request = new RoutesDirectionsRequest();

        request.Key = _apiKeyProvider.Key;
        request.Intermediates = command.Destinations.Map(e => new RouteWayPoint { Address = e });
        request.Origin = new RouteWayPoint {Address = command.origin };
        request.Destination = new RouteWayPoint {Address = command.origin };
        request.TravelMode = RouteTravelMode.Drive;
        request.OptimizeWaypointOrder = true;
        
        var res = await GoogleApi.GoogleMaps.Routes.RouteDirections.QueryAsync(request);
        
        if (res == null || res.Error.Status != Status.Ok)
        {
            return new Error();
        }
        
        if (!res.Routes.Any())
        {
            return new Error();
        }

        return new RoutePackagesCommand.Result(res.Routes.GetFirst()
            .OptimizedIntermediateWaypointIndex.Map(e => command.Destinations[e]).ToList());
    }
}