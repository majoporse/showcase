using System.Linq.Expressions;

namespace Infrastructure.QueryObjects;

public interface IQueryObject<TEntity> where TEntity : class
{
    IQueryObject<TEntity> Filter(Expression<Func<TEntity, bool>> filter);

    IQueryObject<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false);

    IQueryObject<TEntity> OrderBy<TKey>(
        params (Expression<Func<TEntity, TKey>> KeySelector, bool IsDesc)[] keySelectors);

    IQueryObject<TEntity> Paginate(int pageIndex, int pageSize);
    IQueryObject<TEntity> Include(params string[] includes);
    Task<FilteredResult<TEntity>> ExecuteAsync();
}