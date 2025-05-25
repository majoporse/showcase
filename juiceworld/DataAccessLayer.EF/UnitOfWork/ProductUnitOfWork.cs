using Infrastructure.Repositories;
using JuiceWorld.Data;
using JuiceWorld.Entities;
using JuiceWorld.Repositories;

namespace JuiceWorld.UnitOfWork;

public class ProductUnitOfWork
{
    private readonly JuiceWorldDbContext _context;
    public readonly IRepository<Product> ProductRepository;
    public readonly IRepository<Tag> TagRepository;

    public ProductUnitOfWork(JuiceWorldDbContext context)
    {
        _context = context;
        ProductRepository = new Repository<Product>(_context);
        TagRepository = new Repository<Tag>(_context);
    }

    public ProductUnitOfWork(IRepository<Product> productRepository, IRepository<Tag> tagRepository) //for stub testing
    {
        ProductRepository = productRepository;
        TagRepository = tagRepository;
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
