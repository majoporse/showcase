namespace BusinessLayer.DTOs;

public class ReviewDetailDto : BaseEntityDto
{
    public int Rating { get; set; }
    public required string Body { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public required UserDto User { get; set; }
}