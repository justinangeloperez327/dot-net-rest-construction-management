using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Activities;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ActivityRepository(ApplicationDbContext dbContext)
    : IActivityRepository
{
    public Task<Activity?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Activities.SingleOrDefaultAsync(
            activity => activity.Id == id,
            cancellationToken);

    public Task<bool> ExistsByCodeAsync(
        Guid projectId,
        string normalizedCode,
        CancellationToken cancellationToken = default) =>
        dbContext.Activities.AnyAsync(
            activity =>
                activity.ProjectId == projectId
                && activity.NormalizedCode == normalizedCode,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Activity> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Activity> query = dbContext.Activities
            .AsNoTracking()
            .Where(activity => activity.ProjectId == projectId)
            .OrderBy(activity => activity.Code);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Activity[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        dbContext.Activities.Add(activity);
    }
}
