using Construction.Application.Common.Pagination;
using Construction.Domain.Inspections;

namespace Construction.Application.Abstractions.Data;

public interface IInspectionRepository
{
    Task<Inspection?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Inspection> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Inspection inspection);
}
