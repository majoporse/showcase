using ErrorOr;
using Microsoft.AspNetCore.Http;
using Warehouse.Application.EndpointHandlers.Courier;
using Wolverine;
using Wolverine.Http;

namespace Warehouse.Presentation.Courier;

public record AddCourierRequest(string Name, string Email)
{
    public record Response(Guid CourierId, string Name, string Email, string Status);
}

public class AddCourierEndpoint
{
    [Tags("Warehouse - Courier")]
    [WolverinePost("/addCourier")]
    public static async Task<IResult> AddCouriersAsync(AddCourierRequest request, IMessageBus sender)
    {
        var command = new AddCourierCommand(request.Name, request.Email);

        var result = await sender.InvokeAsync<ErrorOr<AddCourierCommand.Result>>(command);

        return result.Match(
            value => Results.Ok(
                new AddCourierRequest.Response(
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