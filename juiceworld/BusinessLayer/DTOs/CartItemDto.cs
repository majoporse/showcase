namespace BusinessLayer.DTOs;

public class CartItemDto : BaseEntityDto
{
    public int Quantity { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
}