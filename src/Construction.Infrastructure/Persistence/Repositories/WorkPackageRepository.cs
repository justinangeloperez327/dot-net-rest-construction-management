using Construction.Application.Abstractions.Data;
using Construction.Domain.WorkPackages;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class WorkPackageRepository(ApplicationDbContext dbContext)
    : IWorkPackageRepository
{
    public Task<WorkPackage?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.WorkPackages.SingleOrDefaultAsync(
            workPackage => workPackage.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<WorkPackage>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.WorkPackages
            .AsNoTracking()
            .Where(workPackage => workPackage.ProjectId == projectId)
            .OrderBy(workPackage => workPackage.Code)
            .ToArrayAsync(cancellationToken);

    public Task<bool> ExistsByCodeAsync(
        Guid projectId,
        string normalizedCode,
        CancellationToken cancellationToken = default) =>
        dbContext.WorkPackages.AnyAsync(
            workPackage =>
                workPackage.ProjectId == projectId
                && workPackage.NormalizedCode == normalizedCode,
            cancellationToken);

    public async Task<bool> WouldCreateCycleAsync(
        Guid workPackageId,
        Guid parentWorkPackageId,
        CancellationToken cancellationToken = default)
    {
        Guid? current = parentWorkPackageId;
        var visited = new HashSet<Guid>();

        while (current is Guid currentId)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (currentId == workPackageId)
            {
                return true;
            }

            if (!visited.Add(currentId))
            {
                return true;
            }

            current = await dbContext.WorkPackages
                .AsNoTracking()
                .Where(workPackage => workPackage.Id == currentId)
                .Select(workPackage => workPackage.ParentWorkPackageId)
                .SingleOrDefaultAsync(cancellationToken);
        }

        return false;
    }

    public void Add(WorkPackage workPackage)
    {
        ArgumentNullException.ThrowIfNull(workPackage);
        dbContext.WorkPackages.Add(workPackage);
    }
}
