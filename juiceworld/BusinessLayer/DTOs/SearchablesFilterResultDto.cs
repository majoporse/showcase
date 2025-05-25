using Infrastructure.QueryObjects;

namespace BusinessLayer.DTOs;

public class SearchablesFilterResultDto
{
    public required FilteredResult<ProductDetailDto> Products { get; set; }
    public required FilteredResult<ManufacturerDto> Manufacturers { get; set; }
    public required FilteredResult<TagDto> Tags { get; set; }
}
