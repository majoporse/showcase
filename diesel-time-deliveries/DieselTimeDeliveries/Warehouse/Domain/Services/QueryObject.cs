using System.Linq.Expressions;

namespace Warehouse.Domain.Services;

public interface IQueryObject<TAggregate> where TAggregate : class
{
    IQueryObject<TAggregate> Filter(Expression<Func<TAggregate, bool>> predicate);
    IQueryObject<TAggregate> OrderBy(Expression<Func<TAggregate, object>> selector, bool ascending = true);
    IQueryObject<TAggregate> Page(int page, int pageSize);

    Task<IEnumerable<TAggregate>> ExecuteAsync();
}

public abstract class QueryObject<TAggregate> : IQueryObject<TAggregate> where TAggregate : class
{
    protected IQueryable<TAggregate> _query;
    protected List<(Expression<Func<TAggregate, object>> selector, bool ascending)> _sortingCriteria = [];

    public IQueryObject<TAggregate> Filter(Expression<Func<TAggregate, bool>> predicate)
    {
        _query = _query.Where(predicate);
        return this;
    }

    public IQueryObject<TAggregate> Page(int page, int pageSize)
    {
        _query = _query.Skip((page - 1) * pageSize).Take(pageSize);
        return this;
    }

    public IQueryObject<TAggregate> OrderBy(Expression<Func<TAggregate, object>> selector, bool ascending = true)
    {
        _sortingCriteria.Add((selector, ascending));
        _query = ApplySorting();
        return this;
    }

    public abstract Task<IEnumerable<TAggregate>> ExecuteAsync();

    protected IQueryable<TAggregate> ApplySorting()
    {
        if (_sortingCriteria.Count == 0)
            return _query;

        IOrderedQueryable<TAggregate> orderedQuery = null;

        foreach (var criteria in _sortingCriteria)
            if (orderedQuery == null)
                orderedQuery = criteria.ascending
                    ? _query.OrderBy(criteria.selector)
                    : _query.OrderByDescending(criteria.selector);
            else
                orderedQuery = criteria.ascending
                    ? orderedQuery.ThenBy(criteria.selector)
                    : orderedQuery.ThenByDescending(criteria.selector);

        return orderedQuery!;
    }
}