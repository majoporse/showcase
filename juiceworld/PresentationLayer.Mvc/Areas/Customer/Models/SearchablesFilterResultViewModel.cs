using Infrastructure.QueryObjects;
using PresentationLayer.Mvc.Areas.Admin.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class SearchablesFilterResultViewModel
{
    public required FilteredResult<ProductDetailViewModel> Products { get; set; }
    public required FilteredResult<ManufacturerViewModel> Manufacturers { get; set; }
    public required FilteredResult<TagViewModel> Tags { get; set; }
}
