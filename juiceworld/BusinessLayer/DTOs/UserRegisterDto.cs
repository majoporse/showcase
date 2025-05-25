using System.ComponentModel.DataAnnotations;
using Commons.Enums;

namespace BusinessLayer.DTOs;

public class UserRegisterDto
{
    [Required]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{L}\p{N}\p{P}\p{S}])[^\s]{8,}$",
        ErrorMessage =
            "Passwords must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; }

    [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
    [Display(Name = "Bio")]
    public required string Bio { get; set; }

    [Display(Name = "User Role")]
    public UserRole UserRole { get; set; } = UserRole.Customer;
}