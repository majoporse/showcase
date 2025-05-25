using System.Linq.Expressions;
using Infrastructure.QueryObjects;
using JuiceWorld.Data;
using JuiceWorld.Entities.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JuiceWorld.QueryObjects;

public class QueryObject<TEntity>(JuiceWorldDbContext context) : IQueryObject<TEntity>
    where TEntity : class, IBaseEntity
{
    private bool _orderingEnabled;
    private int _pageIndex = 1;
    private int _pageSize = 10;
    private bool _pagingEnabled;
    private IQueryable<TEntity> _query = context.Set<TEntity>();

    public IQueryObject<TEntity> Filter(Expression<Func<TEntity, bool>> filter)
    {
        _query = _query.Where(filter);
        return this;
    }

    public IQueryObject<TEntity> OrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false)
    {
        return OrderBy((keySelector, isDesc));
    }

    public IQueryObject<TEntity> OrderBy<TKey>(
        params (Expression<Func<TEntity, TKey>> KeySelector, bool IsDesc)[] keySelectors)
    {
        foreach (var (keySelector, isDesc) in keySelectors)
            if (_orderingEnabled)
            {
                _query = isDesc
                    ? ((IOrderedQueryable<TEntity>)_query).ThenByDescending(keySelector)
                    : ((IOrderedQueryable<TEntity>)_query).ThenBy(keySelector);
            }
            else
            {
                _query = isDesc ? _query.OrderByDescending(keySelector) : _query.OrderBy(keySelector);
                _orderingEnabled = true;
            }

        return this;
    }

    public IQueryObject<TEntity> Paginate(int pageIndex, int pageSize)
    {
        _pagingEnabled = true;
        _pageIndex = pageIndex;
        _pageSize = pageSize;
        return this;
    }

    public IQueryObject<TEntity> Include(params string[] includes)
    {
        _query = includes.Aggregate(_query, (current, include) => current.Include(include));
        return this;
    }

    public async Task<FilteredResult<TEntity>> ExecuteAsync()
    {
        var totalEntities = await _query.CountAsync();
        var entities = _pagingEnabled
            ? await _query.Skip((_pageIndex - 1) * _pageSize).Take(_pageSize).ToListAsync()
            : await _query.ToListAsync();

        return new FilteredResult<TEntity>
        {
            Entities = entities,
            PageIndex = (_pageIndex - 1) * _pageSize > totalEntities ? 1 : _pageIndex,
            TotalPages = (int)Math.Ceiling(totalEntities / (double)_pageSize)
        };
    }
}