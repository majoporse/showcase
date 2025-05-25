using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class UserLoginViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public required string Password { get; set; }
}
