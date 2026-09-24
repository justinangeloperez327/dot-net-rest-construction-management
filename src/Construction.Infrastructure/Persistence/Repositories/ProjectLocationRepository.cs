using Construction.Application.Abstractions.Data;
using Construction.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ProjectLocationRepository(ApplicationDbContext dbContext)
    : IProjectLocationRepository
{
    public Task<ProjectLocation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectLocations.SingleOrDefaultAsync(
            location => location.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<ProjectLocation>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectLocations
            .AsNoTracking()
            .Where(location => location.ProjectId == projectId)
            .OrderBy(location => location.Name)
            .ToArrayAsync(cancellationToken);

    public Task<bool> ExistsByNameAsync(
        Guid projectId,
        string normalizedName,
        Guid? parentLocationId,
        Guid? excludingLocationId = null,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectLocations.AnyAsync(
            location =>
                location.ProjectId == projectId
                && location.ParentLocationId == parentLocationId
                && location.NormalizedName == normalizedName
                && (!excludingLocationId.HasValue
                    || location.Id != excludingLocationId.Value),
            cancellationToken);

    public void Add(ProjectLocation location)
    {
        ArgumentNullException.ThrowIfNull(location);
        dbContext.ProjectLocations.Add(location);
    }
}
