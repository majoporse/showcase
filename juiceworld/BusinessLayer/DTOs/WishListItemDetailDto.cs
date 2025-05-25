namespace BusinessLayer.DTOs;

public class WishListItemDetailDto
{
    public int Id { get; set; }
    public required UserDto User { get; set; }
    public required ProductDto Product { get; set; }
}