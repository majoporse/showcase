using System.Linq.Expressions;
using Infrastructure.Repositories;
using JuiceWorld.Data;
using JuiceWorld.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JuiceWorld.Repositories;

public class Repository<TEntity>(JuiceWorldDbContext context) : IRepository<TEntity>
    where TEntity : class, IBaseEntity
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();

    public async Task<TEntity?> CreateAsync(TEntity entity, object? userId = null, bool saveChanges = true)
    {
        var result = await _dbSet.AddAsync(entity);

        if (saveChanges)
        {
            if (userId is null)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync((int)userId);
            }
        }

        return result.Entity;
    }

    public async Task<bool> CreateRangeAsync(IEnumerable<TEntity> entities, object? userId = null, bool saveChanges = true)
    {
        await _dbSet.AddRangeAsync(entities);

        if (saveChanges)
        {
            if (userId is null)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync((int)userId);
            }
        }

        return true;
    }

    public Task<TEntity?> GetByIdAsync(object id, params string[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return query.FirstOrDefaultAsync(e => e.Id == (int)id);
    }

    public Task<List<TEntity>> GetAllAsync(params string[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return query.ToListAsync();
    }

    public Task<List<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate,
        params string[] includes)
    {
        var query = includes.Aggregate(_dbSet.AsQueryable(), (current, include) => current.Include(include));
        return query.Where(predicate).ToListAsync();
    }

    public Task<List<TEntity>> GetByIdRangeAsync(IEnumerable<object> ids)
    {
        return _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity, object? userId = null, bool saveChanges = true)
    {
        var existingEntity = await _dbSet.FindAsync(entity.Id);
        if (existingEntity != null) context.Entry(existingEntity).State = EntityState.Detached;

        var result = _dbSet.Update(entity);

        if (saveChanges)
        {
            if (userId is null)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync((int)userId);
            }
        }

        return result.Entity;
    }

    public async Task<bool> DeleteAsync(object id, object? userId = null, bool saveChanges = true)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity is null) return false;

        _dbSet.Remove(entity);

        if (saveChanges)
        {
            if (userId is null)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync((int)userId);
            }
        }

        return true;
    }

    public async Task<int> RemoveAllByConditionAsync(Expression<Func<TEntity, bool>> predicate, object? userId = null, bool saveChanges = true)
    {
        var entities = await _dbSet.Where(predicate).ToListAsync();
        _dbSet.RemoveRange(entities);

        if (saveChanges)
        {
            if (userId is null)
            {
                await context.SaveChangesAsync();
            }
            else
            {
                await context.SaveChangesAsync((int)userId);
            }
        }

        return entities.Count;
    }
}
