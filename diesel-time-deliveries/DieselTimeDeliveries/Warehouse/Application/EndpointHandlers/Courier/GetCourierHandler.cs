using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Courier;

public record GetCourierQuery(Guid CourierId)
{
    public record Result(Guid CourierId, string Name, string Email, string Status);
}

public class GetCourierHandler(IQueryObject<Domain.Models.Courier.Courier> queryObject)
{
    public async Task<ErrorOr<GetCourierQuery.Result>> Handle(GetCourierQuery query)
    {
        var courier = (await queryObject.Filter(p => p.Id == CourierId.Create(query.CourierId)).ExecuteAsync())
            .SingleOrDefault();

        if (courier is null)
            return Error.Validation($"Courier (id: {query.CourierId}) not found");

        return new GetCourierQuery.Result(
            courier.Id.Value,
            courier.Name,
            courier.Email.Value,
            courier.Status.ToString()
        );
    }
}