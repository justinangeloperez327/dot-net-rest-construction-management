using Construction.Application.Common.Pagination;
using Construction.Domain.Companies;

namespace Construction.Application.Abstractions.Data;

public interface ICompanyRepository
{
    Task<Company?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string normalizedName,
        Guid? excludingCompanyId = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Company> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Company company);
}
