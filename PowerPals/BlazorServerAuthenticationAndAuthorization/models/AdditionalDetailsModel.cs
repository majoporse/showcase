using System.ComponentModel.DataAnnotations;

namespace BlazorServerAuthenticationAndAuthorization.models
{
	public class AdditionalDetailsModel
	{
		[Required]
		//[StringLength(420, ErrorMessage = "Name length can't be more than 420.")]
		public string bio { get; set; } = "";

		[Required]
		//[Url(ErrorMessage = "Invalid URL")]
		public string ImageUrl { get; set; } = "";
	}
}
