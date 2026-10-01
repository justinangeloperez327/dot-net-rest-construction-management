using Construction.Application.Common.Pagination;
using Construction.Domain.Issues;

namespace Construction.Application.Abstractions.Data;

public interface IIssueRepository
{
    Task<Issue?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Issue> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Issue issue);
}
