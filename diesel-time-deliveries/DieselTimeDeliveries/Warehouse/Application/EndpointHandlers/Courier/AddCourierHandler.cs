using ErrorOr;
using Warehouse.Domain.Services;

namespace Warehouse.Application.EndpointHandlers.Courier;

public record AddCourierCommand(string Name, string Email)
{
    public record Result(Guid CourierId, string Name, string Email, string Status);
}

public class AddCourierHandler(IRepository<Domain.Models.Courier.Courier> repository)
{
    public async Task<ErrorOr<AddCourierCommand.Result>> HandleAsync(AddCourierCommand command)
    {
        var goods = Domain.Models.Courier.Courier.Create(command.Name, command.Email);

        if (goods.IsError)
            return goods.Errors;

        var addedGoods = await repository.InsertAsync(goods.Value);

        addedGoods.CourierAdded();

        await repository.CommitAsync();
        return new AddCourierCommand.Result(
            addedGoods.Id.Value,
            addedGoods.Name,
            addedGoods.Email.Value,
            addedGoods.Status.ToString()
        );
    }
}