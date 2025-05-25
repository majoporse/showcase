using Microsoft.AspNetCore.Http;
using Wolverine;
using Wolverine.Http;
using ErrorOr;
using Warehouse.Application.EndpointHandlers.Courier;

namespace Warehouse.Presentation.Courier;

public record DeleteCourierRequest(Guid CourierId)
{
    public record Response();
}

public class DeleteCourierEndpoint
{
    [Tags("Warehouse - Courier")]
    [WolverineDelete("/deleteCourier")]
    public static async Task<IResult> DeleteCouriersAsync(DeleteCourierRequest request, IMessageBus sender)
    {
        var command = new DeleteCourierCommand(request.CourierId);

        var result = await sender.InvokeAsync<ErrorOr<DeleteCourierCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new DeleteCourierRequest.Response()
            ),
            errors => Results.BadRequest(errors.Select(e => e.Code))
        );
    }
}