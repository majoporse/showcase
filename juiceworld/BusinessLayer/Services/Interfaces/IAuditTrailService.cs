using BusinessLayer.DTOs;

namespace BusinessLayer.Services.Interfaces;

public interface IAuditTrailService
{
    public Task<IEnumerable<AuditTrailDto>> GetAuditTrailsFilteredAsync(AuditTrailFilterDto filter);
    public Task<AuditTrailDto?> GetAuditTrailByIdAsync(int id);
}