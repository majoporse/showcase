namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class SearchablesFilterViewModel
{
    public string? NameQuery { get; set; }
    public string? ProductCategory { get; set; }
    public decimal? ProductPriceMax { get; set; }
    public decimal? ProductPriceMin { get; set; }
    public int ProductPageIndex { get; set; } = 1;
    public int ProductPageSize { get; set; } = 10;

    public int? TagId { get; set; }
    public int TagPageIndex { get; set; } = 1;
    public int TagPageSize { get; set; } = 10;

    public int? ManufacturerId { get; set; }
    public int ManufacturerPageIndex { get; set; } = 1;
    public int ManufacturerPageSize { get; set; } = 10;
}
