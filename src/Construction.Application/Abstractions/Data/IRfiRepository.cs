using Construction.Application.Common.Pagination;
using Construction.Domain.Rfis;

namespace Construction.Application.Abstractions.Data;

public interface IRfiRepository
{
    Task<Rfi?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Rfi> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Rfi rfi);
}
