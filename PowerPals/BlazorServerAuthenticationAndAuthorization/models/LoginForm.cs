using System.ComponentModel.DataAnnotations;

namespace BlazorServerAuthenticationAndAuthorization.models;

public class LoginForm
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}
