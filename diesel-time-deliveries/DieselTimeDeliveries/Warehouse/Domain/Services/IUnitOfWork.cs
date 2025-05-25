namespace Warehouse.Domain.Services;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken);
}