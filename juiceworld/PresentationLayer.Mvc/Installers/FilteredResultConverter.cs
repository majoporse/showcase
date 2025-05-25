using AutoMapper;
using Infrastructure.QueryObjects;

namespace PresentationLayer.Mvc.Installers;

public class
    FilteredResultConverter<TSource, TDestination> : ITypeConverter<FilteredResult<TSource>,
    FilteredResult<TDestination>> where TSource : class where TDestination : class
{
    public FilteredResult<TDestination> Convert(FilteredResult<TSource> source,
        FilteredResult<TDestination> destination, ResolutionContext context)
    {
        return new FilteredResult<TDestination>
        {
            Entities = context.Mapper.Map<IEnumerable<TDestination>>(source.Entities),
            PageIndex = source.PageIndex,
            TotalPages = source.TotalPages
        };
    }
}
