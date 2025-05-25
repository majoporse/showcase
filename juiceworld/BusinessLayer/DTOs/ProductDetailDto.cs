using Commons.Enums;

namespace BusinessLayer.DTOs;

public class ProductDetailDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public required string Description { get; set; }
    public string? ImageValue { get; set; }
    public string Imageurl { get; set; } = string.Empty;
    public ProductCategory Category { get; set; }
    public ProductUsageType UsageType { get; set; }

    public required ManufacturerDto Manufacturer { get; set; }
    public List<ReviewDetailDto> Reviews { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
}
