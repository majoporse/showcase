using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Commons.Enums;

namespace JuiceWorld.Entities;

public class AuditTrail : BaseEntity
{
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User? User { get; set; }

    public TrailType TrailType { get; set; }

    [MaxLength(100)]
    public required string EntityName { get; set; }

    public int PrimaryKey { get; set; }

    public List<string> ChangedColumns { get; set; } = [];
}