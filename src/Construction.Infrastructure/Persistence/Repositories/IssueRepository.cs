using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Issues;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class IssueRepository(ApplicationDbContext dbContext)
    : IIssueRepository
{
    public Task<Issue?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Issues
            .AsSplitQuery()
            .Include(issue => issue.CorrectiveActions)
            .Include(issue => issue.History)
            .SingleOrDefaultAsync(
                issue => issue.Id == id,
                cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Issues.AnyAsync(
            issue =>
                issue.ProjectId == projectId
                && issue.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Issue> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Issue> query = dbContext.Issues
            .AsNoTracking()
            .Where(issue => issue.ProjectId == projectId)
            .OrderByDescending(issue => issue.CreatedAtUtc);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Issue[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Issue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);
        dbContext.Issues.Add(issue);
    }
}
