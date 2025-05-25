namespace BusinessLayer.DTOs;

public class CartItemDetailDto : BaseEntityDto
{
    public int Quantity { get; set; }
    public required ProductDto Product { get; set; }
    public int UserId { get; set; }
}