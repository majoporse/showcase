using System.ComponentModel.DataAnnotations;
using Commons.Enums;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class UserSimpleViewModel
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Display(Name = "User Role")]
    public UserRole UserRole { get; set; } = UserRole.Customer;
}
