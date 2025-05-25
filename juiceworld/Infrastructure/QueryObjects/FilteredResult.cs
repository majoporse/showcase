namespace Infrastructure.QueryObjects;

public class FilteredResult<TEntity> where TEntity : class
{
    public IEnumerable<TEntity> Entities { get; set; } = [];
    public int PageIndex { get; set; }
    public int TotalPages { get; set; }
}