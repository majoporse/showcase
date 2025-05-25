using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Courier;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Courier;

public record UpdateCourierRequest(Guid CourierId, string? Name,  string? Email, string? Status)
{
    public record Response(Guid CourierId, string Name, string Email, string Status);
}

public class UpdateCourierEndpoint
{
    [Tags("Warehouse - Courier")]
    [WolverinePatch("/updateCourier")]
    public static async Task<IResult> AddCouriersAsync(UpdateCourierRequest request, IMessageBus sender)
    {
        var command = new UpdateCourierCommand(request.CourierId, request.Name,  request.Email, request.Status);

        var result = await sender.InvokeAsync<ErrorOr<UpdateCourierCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new UpdateCourierRequest.Response(
                    value.CourierId,
                    value.Name,
                    value.Email,
                    value.Status.ToString()
                )
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}