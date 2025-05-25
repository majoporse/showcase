using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Courier;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Courier;

public record GetCourierResponse(Guid CourierId, string Name, string Email, string Status);

public class GetCourierEndpoint
{
    [Tags("Warehouse - Courier")]
    [WolverineGet("/courier/{id}")]
    public static async Task<IResult> GetCourierAsync(Guid id, IMessageBus sender)
    {
        var query = new GetCourierQuery(id);

        var result = await sender.InvokeAsync<ErrorOr<GetCourierQuery.Result>>(query);

        return result.Match(
            value => Results.Ok(
                new GetCourierResponse(
                    value.CourierId,
                    value.Name,
                    value.Email,
                    value.Status
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}