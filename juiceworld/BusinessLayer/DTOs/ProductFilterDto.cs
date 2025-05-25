namespace BusinessLayer.DTOs;

public class ProductFilterDto : PaginationDto
{
    public string? Category { get; set; }
    public string? NameQuery { get; set; }
    public decimal? PriceMax { get; set; }
    public decimal? PriceMin { get; set; }
    public int? TagId { get; set; }
    public int? ManufacturerId { get; set; }
}