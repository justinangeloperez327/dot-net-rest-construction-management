using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Rfis;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class RfiRepository(ApplicationDbContext dbContext)
    : IRfiRepository
{
    public Task<Rfi?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Rfis
            .AsSplitQuery()
            .Include(rfi => rfi.Comments)
            .Include(rfi => rfi.History)
            .SingleOrDefaultAsync(
                rfi => rfi.Id == id,
                cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Rfis.AnyAsync(
            rfi =>
                rfi.ProjectId == projectId
                && rfi.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Rfi> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Rfi> query = dbContext.Rfis
            .AsNoTracking()
            .Where(rfi => rfi.ProjectId == projectId)
            .OrderByDescending(rfi => rfi.CreatedAtUtc);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Rfi[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Rfi rfi)
    {
        ArgumentNullException.ThrowIfNull(rfi);
        dbContext.Rfis.Add(rfi);
    }
}
