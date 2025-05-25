using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class GiftCardFilterViewModel : PaginationViewModel
{
    public string? Name { get; set; }
}
