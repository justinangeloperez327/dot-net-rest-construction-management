using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class CompanyRepository(ApplicationDbContext dbContext)
    : ICompanyRepository
{
    public Task<Company?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Companies.SingleOrDefaultAsync(
            company => company.Id == id,
            cancellationToken);

    public Task<bool> ExistsByNameAsync(
        string normalizedName,
        Guid? excludingCompanyId = null,
        CancellationToken cancellationToken = default) =>
        dbContext.Companies.AnyAsync(
            company =>
                company.NormalizedName == normalizedName
                && (!excludingCompanyId.HasValue
                    || company.Id != excludingCompanyId.Value),
            cancellationToken);

    public async Task<(IReadOnlyCollection<Company> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Company> query = dbContext.Companies
            .AsNoTracking()
            .OrderBy(company => company.Name);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Company[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Company company)
    {
        ArgumentNullException.ThrowIfNull(company);
        dbContext.Companies.Add(company);
    }
}
