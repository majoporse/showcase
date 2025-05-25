using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class ProductFilterViewModel : PaginationViewModel
{
    public string? Category { get; set; }
    public string? NameQuery { get; set; }
    public decimal? PriceMax { get; set; }
    public decimal? PriceMin { get; set; }
    public int? TagId { get; set; }
    public int? ManufacturerId { get; set; }
}
