using Construction.Application.Common.Pagination;
using Construction.Domain.Audit;

namespace Construction.Application.Abstractions.Data;

public interface IAuditRepository
{
    Task<(IReadOnlyCollection<AuditLog> Items, long TotalCount)> GetPageAsync(
        Guid? projectId,
        AuditCategory? category,
        Guid? userId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        PageRequest page,
        CancellationToken cancellationToken = default);
}
