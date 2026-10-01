using Construction.Application.Common.Pagination;
using Construction.Domain.DailyProgress;

namespace Construction.Application.Abstractions.Data;

public interface IDailyProgressRepository
{
    Task<DailyProgressReport?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForDateAsync(
        Guid projectId,
        DateOnly reportDate,
        Guid? excludingReportId = null,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<DailyProgressReport> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(DailyProgressReport report);

    void Remove(DailyProgressReport report);
}
