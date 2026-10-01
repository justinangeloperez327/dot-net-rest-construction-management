using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Audit;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class AuditRepository(ApplicationDbContext dbContext)
    : IAuditRepository
{
    public async Task<(IReadOnlyCollection<AuditLog> Items, long TotalCount)> GetPageAsync(
        Guid? projectId,
        AuditCategory? category,
        Guid? userId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<AuditLog> query = dbContext.AuditLogs
            .AsNoTracking();

        if (projectId is Guid requestedProjectId)
        {
            query = query.Where(audit =>
                audit.ProjectId == requestedProjectId);
        }

        if (category is AuditCategory requestedCategory)
        {
            query = query.Where(audit =>
                audit.Category == requestedCategory);
        }

        if (userId is Guid requestedUserId)
        {
            query = query.Where(audit =>
                audit.UserId == requestedUserId);
        }

        if (fromUtc is DateTimeOffset start)
        {
            query = query.Where(audit =>
                audit.OccurredAtUtc >= start);
        }

        if (toUtc is DateTimeOffset end)
        {
            query = query.Where(audit =>
                audit.OccurredAtUtc <= end);
        }

        query = query.OrderByDescending(
            audit => audit.OccurredAtUtc);

        long totalCount = await query.LongCountAsync(cancellationToken);

        AuditLog[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }
}
