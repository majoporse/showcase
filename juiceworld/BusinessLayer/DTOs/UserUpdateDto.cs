using System.ComponentModel.DataAnnotations;
using Commons.Enums;

namespace BusinessLayer.DTOs;

public class UserUpdateDto
{
    public int Id { get; set; }

    [Display(Name = "Username")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\p{L}\p{N}\p{P}\p{S}])[^\s]{8,}$",
        ErrorMessage =
            "Passwords must have at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string? ConfirmPassword { get; set; }

    [MaxLength(500, ErrorMessage = "Bio cannot exceed 500 characters.")]
    [Display(Name = "Bio")]
    public string? Bio { get; set; }

    [Display(Name = "User Role")]
    public UserRole UserRole { get; set; } = UserRole.Customer;
}