using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Inspections;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class InspectionRepository(ApplicationDbContext dbContext)
    : IInspectionRepository
{
    public Task<Inspection?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Inspections
            .Include(inspection => inspection.History)
            .SingleOrDefaultAsync(
                inspection => inspection.Id == id,
                cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Inspections.AnyAsync(
            inspection =>
                inspection.ProjectId == projectId
                && inspection.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Inspection> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Inspection> query = dbContext.Inspections
            .AsNoTracking()
            .Where(inspection => inspection.ProjectId == projectId)
            .OrderByDescending(inspection => inspection.CreatedAtUtc);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Inspection[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Inspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);
        dbContext.Inspections.Add(inspection);
    }
}
