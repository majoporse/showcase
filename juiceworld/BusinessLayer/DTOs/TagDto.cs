using System.ComponentModel;

namespace BusinessLayer.DTOs;

public class TagDto : BaseEntityDto
{
    public string Name { get; set; } = null!;

    [DisplayName("Color")]
    public string ColorHex { get; set; } = null!;
}