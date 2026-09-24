using Construction.Application.Common.Pagination;
using Construction.Domain.Projects;

namespace Construction.Application.Abstractions.Data;

public interface IProjectRepository
{
    Task<Project?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNumberAsync(
        string normalizedNumber,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Project> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        Guid? userId = null,
        bool includeAllProjects = false,
        CancellationToken cancellationToken = default);

    void Add(Project project);
}
