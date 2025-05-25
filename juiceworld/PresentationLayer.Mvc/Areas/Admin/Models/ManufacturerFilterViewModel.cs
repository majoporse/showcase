using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class ManufacturerFilterViewModel : PaginationViewModel
{
    public string? Name { get; set; }
}
