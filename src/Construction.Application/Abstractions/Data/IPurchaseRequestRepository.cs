using Construction.Application.Common.Pagination;
using Construction.Domain.PurchaseRequests;

namespace Construction.Application.Abstractions.Data;

public interface IPurchaseRequestRepository
{
    Task<PurchaseRequest?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<PurchaseRequest> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(PurchaseRequest request);
}
