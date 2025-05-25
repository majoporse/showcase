using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Interfaces;
using Commons.Enums;
using Infrastructure.QueryObjects;
using Infrastructure.Repositories;
using JuiceWorld.Entities;

namespace BusinessLayer.Services;

public class AuditTrailService(
    IRepository<AuditTrail> auditTrailRepository,
    IQueryObject<AuditTrail> queryObject,
    IMapper mapper) : IAuditTrailService
{
    public async Task<IEnumerable<AuditTrailDto>> GetAuditTrailsFilteredAsync(AuditTrailFilterDto filter)
    {
        Enum.TryParse<TrailType>(filter.TrailType, true, out var trailTypeEnum);
        var query = queryObject.Filter(at =>
                filter.TrailType == null || at.TrailType == trailTypeEnum)
            .Filter(at => filter.TimestampFrom == null || at.CreatedAt >= filter.TimestampFrom)
            .Filter(at => filter.TimestampTo == null || at.CreatedAt <= filter.TimestampTo)
            .Filter(at => filter.EntityName == null || at.EntityName == filter.EntityName)
            .Filter(at => filter.PrimaryKey == null || at.PrimaryKey == filter.PrimaryKey)
            .Filter(at =>
                filter.ChangedColumns == null ||
                filter.ChangedColumns.Any(column => at.ChangedColumns.Contains(column)))
            .Filter(at => filter.UserId == null || at.UserId == filter.UserId)
            .OrderBy(at => at.CreatedAt);

        if (filter is { PageIndex: not null, PageSize: not null })
            query = query.Paginate(filter.PageIndex.Value, filter.PageSize.Value);

        var result = await query.ExecuteAsync();
        return mapper.Map<List<AuditTrailDto>>(result.Entities);
    }

    public async Task<AuditTrailDto?> GetAuditTrailByIdAsync(int id)
    {
        var auditTrail = await auditTrailRepository.GetByIdAsync(id);
        return auditTrail is null ? null : mapper.Map<AuditTrailDto>(auditTrail);
    }
}