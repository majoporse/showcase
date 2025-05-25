using RegistR.Attributes.Base;
using Warehouse.Domain.Services;
using Warehouse.Infrastructure.Persistence;

namespace Warehouse.Infrastructure.Services;

[Register<IUnitOfWork>]
public class EfCoreUnitOfWork : IUnitOfWork
{
    private readonly WarehouseDbContext _context;

    public EfCoreUnitOfWork(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}