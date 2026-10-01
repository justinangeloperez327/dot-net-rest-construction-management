using Construction.Application.Common.Pagination;
using Construction.Domain.Activities;

namespace Construction.Application.Abstractions.Data;

public interface IActivityRepository
{
    Task<Activity?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        Guid projectId,
        string normalizedCode,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Activity> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Activity activity);
}
