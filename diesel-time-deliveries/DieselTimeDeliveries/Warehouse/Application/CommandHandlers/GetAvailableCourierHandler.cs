using Contracts.Queries;
using Warehouse.Domain.Services;
using ErrorOr;
using Warehouse.Domain.Models.Courier;

namespace Warehouse.Application.CommandHandlers;

public class GetAvailableCourierHandler(
    IRepository<Domain.Models.Courier.Courier> repository,
    IQueryObject<Domain.Models.Courier.Courier> queryObject)
{
    public async Task<ErrorOr<GetAvailableCourierQuery.Result>> HandleAsync(GetAvailableCourierQuery query)
    {
        var couriers = (await queryObject.Filter(c => c.Status == CourierStatusEnum.WaitingForAssignment)
            .ExecuteAsync())
            .SingleOrDefault();
        
        if (couriers is null)
        {
            return Error.Validation("No available couriers");
        }
        
        return new GetAvailableCourierQuery.Result(couriers.Id.Value);
    }
}