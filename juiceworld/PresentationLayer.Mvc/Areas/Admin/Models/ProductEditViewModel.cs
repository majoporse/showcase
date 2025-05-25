using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class ProductEditViewModel
{
    public required ProductImageViewModel Product { get; set; }
    public IEnumerable<ManufacturerViewModel> AllManufacturers { get; set; } = [];
    public IEnumerable<TagViewModel> AllTags { get; set; } = [];

    public IFormFile? ImageFile { get; set; }
}

