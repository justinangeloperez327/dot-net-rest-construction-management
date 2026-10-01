using Construction.Application.Common.Pagination;
using Construction.Domain.Suppliers;

namespace Construction.Application.Abstractions.Data;

public interface ISupplierRepository
{
    Task<Supplier?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        string normalizedCode,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Supplier> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Supplier supplier);
}
