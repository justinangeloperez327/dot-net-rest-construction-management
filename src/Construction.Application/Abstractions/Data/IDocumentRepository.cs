using Construction.Application.Common.Pagination;
using Construction.Domain.Documents;

namespace Construction.Application.Abstractions.Data;

public interface IDocumentRepository
{
    Task<ProjectDocument?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<ProjectDocument> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(ProjectDocument document);
}
