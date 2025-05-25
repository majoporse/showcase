using BusinessLayer.DTOs;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class ProductDetailViewModel
{
    public required ProductDetailDto ProductDetail { get; set; }
    public required bool IsInWishList { get; set; }
}
