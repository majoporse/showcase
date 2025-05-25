namespace BusinessLayer.DTOs;

public class OrderProductDetailDto : BaseEntityDto
{
    public int Quantity { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public ProductDto? Product { get; set; }
    public decimal Price { get; set; }
}