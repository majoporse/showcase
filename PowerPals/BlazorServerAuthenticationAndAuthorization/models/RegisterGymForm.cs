using System.ComponentModel.DataAnnotations;

namespace BlazorServerAuthenticationAndAuthorization.models
{
    public class RegisterGymForm
    {
        [Required]
        [StringLength(420, ErrorMessage = "Name length can't be more than 420.")]
        public string desc { get; set; } = "";

        [Required]
        [Url(ErrorMessage = "Invalid URL")]
        public string ImageUrl { get; set; } = "";

        [Required]
        [StringLength(69, ErrorMessage = "Name length can't be more than 69.")]
        public string Name { get; set; } = "";
    }
}
