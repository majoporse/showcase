using PresentationLayer.Mvc.Areas.Admin.Models;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Customer.Models;

public class WishlistItemDetailViewModel
{
    public int Id { get; set; }
    public required UserSimpleViewModel User { get; set; }
    public required ProductViewModel Product { get; set; }
}
