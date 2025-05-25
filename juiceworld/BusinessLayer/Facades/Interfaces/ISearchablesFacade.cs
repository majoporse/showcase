using BusinessLayer.DTOs;

namespace BusinessLayer.Facades.Interfaces;

public interface ISearchablesFacade
{
    Task<SearchablesFilterResultDto> GetSearchablesFilteredAsync(SearchablesFilterDto searchablesFilter);
}