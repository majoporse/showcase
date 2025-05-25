using System.ComponentModel;
using PresentationLayer.Mvc.Models;

namespace PresentationLayer.Mvc.Areas.Admin.Models;

public class TagViewModel : BaseEntityViewModel
{
    public string Name { get; set; } = null!;

    [DisplayName("Color")]
    public string ColorHex { get; set; } = null!;
}
