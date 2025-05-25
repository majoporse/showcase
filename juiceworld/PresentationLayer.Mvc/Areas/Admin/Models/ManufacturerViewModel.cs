using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class ManufacturerViewModel : BaseEntityViewModel
{
    public required string Name { get; set; }
}
