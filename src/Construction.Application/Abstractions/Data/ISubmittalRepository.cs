using Construction.Application.Common.Pagination;
using Construction.Domain.Submittals;

namespace Construction.Application.Abstractions.Data;

public interface ISubmittalRepository
{
    Task<Submittal?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Submittal> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Submittal submittal);
}
