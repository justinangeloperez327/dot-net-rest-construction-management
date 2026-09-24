using Construction.Domain.Locations;

namespace Construction.Application.Abstractions.Data;

public interface IProjectLocationRepository
{
    Task<ProjectLocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProjectLocation>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        Guid projectId,
        string normalizedName,
        Guid? parentLocationId,
        Guid? excludingLocationId = null,
        CancellationToken cancellationToken = default);

    void Add(ProjectLocation location);
}
