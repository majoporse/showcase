using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class CartItemDetailViewModel : BaseEntityViewModel
{
    public int Quantity { get; set; }
    public required ProductViewModel Product { get; set; }
    public int UserId { get; set; }
}
