using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.PurchaseOrders;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderRepository(ApplicationDbContext dbContext)
    : IPurchaseOrderRepository
{
    public Task<PurchaseOrder?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.PurchaseOrders
            .Include(order => order.Items)
            .SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.PurchaseOrders.AnyAsync(
            order =>
                order.ProjectId == projectId
                && order.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public Task<bool> ExistsForPurchaseRequestAsync(
        Guid purchaseRequestId,
        CancellationToken cancellationToken = default) =>
        dbContext.PurchaseOrders.AnyAsync(
            order => order.PurchaseRequestId == purchaseRequestId,
            cancellationToken);

    public async Task<(IReadOnlyCollection<PurchaseOrder> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(page.PageSize, 1, PageRequest.MaximumPageSize);

        IQueryable<PurchaseOrder> query = dbContext.PurchaseOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.ProjectId == projectId)
            .OrderByDescending(order => order.CreatedAtUtc);

        long totalCount = await query.LongCountAsync(cancellationToken);
        PurchaseOrder[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(PurchaseOrder order) => dbContext.PurchaseOrders.Add(order);
}
