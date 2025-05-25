namespace BusinessLayer.DTOs;

public class AuditTrailFilterDto
{
    public string? TrailType { get; set; }
    public DateTime? TimestampFrom { get; set; }
    public DateTime? TimestampTo { get; set; }
    public string? EntityName { get; set; }
    public int? PrimaryKey { get; set; }
    public int? UserId { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public IEnumerable<string>? ChangedColumns { get; set; }
}