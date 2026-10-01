using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Suppliers;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class SupplierRepository(ApplicationDbContext dbContext)
    : ISupplierRepository
{
    public Task<Supplier?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.SingleOrDefaultAsync(supplier => supplier.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(
        string normalizedCode,
        CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.AnyAsync(
            supplier => supplier.NormalizedCode == normalizedCode,
            cancellationToken);

    public Task<bool> ExistsByCompanyAsync(
        Guid companyId,
        CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.AnyAsync(
            supplier => supplier.CompanyId == companyId,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Supplier> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(page.PageSize, 1, PageRequest.MaximumPageSize);

        IQueryable<Supplier> query = dbContext.Suppliers
            .AsNoTracking()
            .OrderBy(supplier => supplier.Code);

        long totalCount = await query.LongCountAsync(cancellationToken);
        Supplier[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Supplier supplier) => dbContext.Suppliers.Add(supplier);
}
