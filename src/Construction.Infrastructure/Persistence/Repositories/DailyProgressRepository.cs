using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.DailyProgress;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class DailyProgressRepository(ApplicationDbContext dbContext)
    : IDailyProgressRepository
{
    public Task<DailyProgressReport?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.DailyProgressReports
            .Include(report => report.Activities)
            .Include(report => report.Manpower)
            .Include(report => report.Equipment)
            .SingleOrDefaultAsync(
                report => report.Id == id,
                cancellationToken);

    public Task<bool> ExistsForDateAsync(
        Guid projectId,
        DateOnly reportDate,
        Guid? excludingReportId = null,
        CancellationToken cancellationToken = default) =>
        dbContext.DailyProgressReports.AnyAsync(
            report =>
                report.ProjectId == projectId
                && report.ReportDate == reportDate
                && (!excludingReportId.HasValue
                    || report.Id != excludingReportId.Value),
            cancellationToken);

    public async Task<(IReadOnlyCollection<DailyProgressReport> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<DailyProgressReport> query =
            dbContext.DailyProgressReports
                .AsNoTracking()
                .Where(report => report.ProjectId == projectId)
                .OrderByDescending(report => report.ReportDate);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        DailyProgressReport[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(DailyProgressReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        dbContext.DailyProgressReports.Add(report);
    }

    public void Remove(DailyProgressReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        dbContext.DailyProgressReports.Remove(report);
    }
}
