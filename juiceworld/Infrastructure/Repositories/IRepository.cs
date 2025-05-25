using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public interface IRepository<TEntity>
    where TEntity : class
{
    Task<TEntity?> CreateAsync(TEntity entity, object? userId = null, bool saveChanges = true);
    Task<bool> CreateRangeAsync(IEnumerable<TEntity> entities, object? userId = null, bool saveChanges = true);
    Task<TEntity?> GetByIdAsync(object id, params string[] includes);
    Task<List<TEntity>> GetAllAsync(params string[] includes);
    Task<List<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate, params string[] includes);
    Task<List<TEntity>> GetByIdRangeAsync(IEnumerable<object> ids);
    Task<TEntity?> UpdateAsync(TEntity entity, object? userId = null, bool saveChanges = true);
    Task<bool> DeleteAsync(object id, object? userId = null, bool saveChanges = true);
    Task<int> RemoveAllByConditionAsync(Expression<Func<TEntity, bool>> predicate, object? userId = null, bool saveChanges = true);
}
