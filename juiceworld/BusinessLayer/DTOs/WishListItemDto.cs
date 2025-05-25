namespace BusinessLayer.DTOs;

public class WishListItemDto : BaseEntityDto
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
}
