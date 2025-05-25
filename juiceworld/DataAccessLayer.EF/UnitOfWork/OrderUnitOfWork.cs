using Infrastructure.Repositories;
using JuiceWorld.Data;
using JuiceWorld.Entities;
using JuiceWorld.Repositories;

namespace JuiceWorld.UnitOfWork;

public class OrderUnitOfWork
{
    private readonly JuiceWorldDbContext _context;
    public readonly IRepository<CartItem> CartItemRepository;
    public readonly IRepository<OrderProduct> OrderProductRepository;
    public readonly IRepository<Order> OrderRepository;
    public readonly IRepository<Product> ProductRepository;
    public readonly IRepository<WishListItem> WishListItemRepository;

    public OrderUnitOfWork(JuiceWorldDbContext context)
    {
        _context = context;
        OrderRepository = new Repository<Order>(_context);
        CartItemRepository = new Repository<CartItem>(_context);
        OrderProductRepository = new Repository<OrderProduct>(_context);
        ProductRepository = new Repository<Product>(_context);
        WishListItemRepository = new Repository<WishListItem>(_context);
    }

    public OrderUnitOfWork() //for stub testing
    {
    }

    public async Task Commit(object? userId = null)
    {
        if (userId is null)
        {
            await _context.SaveChangesAsync();
        }
        else
        {
            await _context.SaveChangesAsync((int)userId);
        }
    }
}
