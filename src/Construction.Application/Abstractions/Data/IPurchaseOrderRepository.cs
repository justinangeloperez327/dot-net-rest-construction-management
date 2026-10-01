using Construction.Application.Common.Pagination;
using Construction.Domain.PurchaseOrders;

namespace Construction.Application.Abstractions.Data;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForPurchaseRequestAsync(
        Guid purchaseRequestId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<PurchaseOrder> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(PurchaseOrder order);
}
