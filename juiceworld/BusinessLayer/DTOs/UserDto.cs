using Commons.Enums;

namespace BusinessLayer.DTOs;

public class UserDto : BaseEntityDto
{
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public UserRole UserRole { get; set; } = UserRole.Customer;
    public string Bio { get; set; } = "";
    public int CouponCodeInCartId { get; set; }
}