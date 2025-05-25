using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Courier;

public record DeleteCourierCommand(Guid CourierId)
{
    public record Result();
}

public class DeleteCourierHandler(
    IRepository<Domain.Models.Courier.Courier> repository,
    IQueryObject<Domain.Models.Courier.Courier> queryObject)
{
    public async Task<ErrorOr<DeleteCourierCommand.Result>> HandleAsync(DeleteCourierCommand command)
    {
        var courier = (await queryObject.Filter(p => p.Id == CourierId.Create(command.CourierId)).ExecuteAsync())
            .SingleOrDefault();

        if (courier is null)
        {
            return Error.NotFound("Courier not found");
        }

        var courierId = CourierId.Create(command.CourierId);

        courier.CourierDeleted();
        
        await repository.RemoveAsync(courierId);
        await repository.CommitAsync();

        return new DeleteCourierCommand.Result();
    }
}