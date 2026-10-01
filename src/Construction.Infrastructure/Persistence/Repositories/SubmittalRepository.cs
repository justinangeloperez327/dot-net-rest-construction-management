using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Submittals;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class SubmittalRepository(ApplicationDbContext dbContext)
    : ISubmittalRepository
{
    public Task<Submittal?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Submittals
            .AsSplitQuery()
            .Include(submittal => submittal.Revisions)
            .Include(submittal => submittal.Comments)
            .Include(submittal => submittal.History)
            .SingleOrDefaultAsync(
                submittal => submittal.Id == id,
                cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Submittals.AnyAsync(
            submittal =>
                submittal.ProjectId == projectId
                && submittal.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Submittal> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Submittal> query = dbContext.Submittals
            .AsNoTracking()
            .Where(submittal => submittal.ProjectId == projectId)
            .OrderByDescending(submittal => submittal.CreatedAtUtc);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Submittal[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Submittal submittal)
    {
        ArgumentNullException.ThrowIfNull(submittal);
        dbContext.Submittals.Add(submittal);
    }
}
