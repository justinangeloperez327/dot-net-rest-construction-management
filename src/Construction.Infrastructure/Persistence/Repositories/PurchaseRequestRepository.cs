using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.PurchaseRequests;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class PurchaseRequestRepository(ApplicationDbContext dbContext)
    : IPurchaseRequestRepository
{
    public Task<PurchaseRequest?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.PurchaseRequests
            .Include(request => request.Items)
            .SingleOrDefaultAsync(request => request.Id == id, cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.PurchaseRequests.AnyAsync(
            request =>
                request.ProjectId == projectId
                && request.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<PurchaseRequest> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(page.PageSize, 1, PageRequest.MaximumPageSize);

        IQueryable<PurchaseRequest> query = dbContext.PurchaseRequests
            .AsNoTracking()
            .Include(request => request.Items)
            .Where(request => request.ProjectId == projectId)
            .OrderByDescending(request => request.CreatedAtUtc);

        long totalCount = await query.LongCountAsync(cancellationToken);
        PurchaseRequest[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(PurchaseRequest request) => dbContext.PurchaseRequests.Add(request);
}
