namespace BusinessLayer.DTOs;

public class ReviewDto : BaseEntityDto
{
    public int Rating { get; set; }
    public required string Body { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
}