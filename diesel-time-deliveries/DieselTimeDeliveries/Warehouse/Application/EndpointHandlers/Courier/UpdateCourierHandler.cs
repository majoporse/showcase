using ErrorOr;
using Warehouse.Domain.Models.Courier;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Courier;

public record UpdateCourierCommand(Guid CourierId, string? Name, string? Email, string? Status)
{
    public record Result(Guid CourierId, string Name, string Email, string Status);
}

public class updateCourierHandler(IRepository<Domain.Models.Courier.Courier> repository, IQueryObject<Domain.Models.Courier.Courier> queryObject)
{
    public async Task<ErrorOr<UpdateCourierCommand.Result>> HandleAsync(UpdateCourierCommand command)
    {
        var courier = 
            (await queryObject.Filter(g => 
                g.Id == CourierId.Create(command.CourierId)).ExecuteAsync())
            .SingleOrDefault();
        
        if (courier is null)
            return Error.Validation("courier not found");

        
        var updateResult = courier.Update(command.Name,  command.Email, command.Status);
        
        if (updateResult.IsError)
            return updateResult.Errors;
        
        var updateedGoods = courier;
        
        courier.CourierUpdated();

        repository.Update(updateedGoods);
        await repository.CommitAsync();
        
        return new UpdateCourierCommand.Result(
            updateedGoods.Id.Value,
            updateedGoods.Name,
            updateedGoods.Email.Value,
            updateedGoods.Status.ToString()
        );
    }
}