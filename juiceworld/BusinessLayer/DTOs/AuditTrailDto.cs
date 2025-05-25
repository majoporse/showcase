using Commons.Enums;

namespace BusinessLayer.DTOs;

public class AuditTrailDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public TrailType TrailType { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string EntityName { get; set; }
    public int PrimaryKey { get; set; }
    public List<string> ChangedColumns { get; set; } = [];
}