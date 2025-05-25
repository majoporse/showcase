using System.ComponentModel.DataAnnotations;

namespace JuiceWorld.Entities;

public class Tag : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [MaxLength(7)]
    public string ColorHex { get; set; } = null!;

    public virtual IEnumerable<Product> Products { get; set; } = null!;
}