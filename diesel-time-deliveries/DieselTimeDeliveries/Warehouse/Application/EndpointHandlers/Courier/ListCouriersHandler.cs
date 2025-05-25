using ErrorOr;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Courier;

public record ListCouriersQuery(int Page, int PageSize)
{
    public record Result(IEnumerable<Courier> Couriers);

    public record Courier(Guid CourierId, string Name, string Email, string Status);
}

public class ListCouriersHandler(IQueryObject<Domain.Models.Courier.Courier> queryObject)
{
    public async Task<ErrorOr<ListCouriersQuery.Result>> Handle(ListCouriersQuery query)
    {
        var couriers = await queryObject.Page(query.Page, query.PageSize).ExecuteAsync();

        return new ListCouriersQuery.Result(
            couriers.Select(p => new ListCouriersQuery.Courier(
                p.Id.Value,
                p.Name,
                p.Email.Value,
                p.Status.ToString()
            ))
        );
    }
}