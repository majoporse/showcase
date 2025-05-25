using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class UserFilterViewModel : PaginationViewModel
{
    public string? Name { get; set; }
}
