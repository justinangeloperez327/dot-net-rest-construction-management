using Construction.Domain.WorkPackages;

namespace Construction.Application.Abstractions.Data;

public interface IWorkPackageRepository
{
    Task<WorkPackage?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<WorkPackage>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(
        Guid projectId,
        string normalizedCode,
        CancellationToken cancellationToken = default);

    Task<bool> WouldCreateCycleAsync(
        Guid workPackageId,
        Guid parentWorkPackageId,
        CancellationToken cancellationToken = default);

    void Add(WorkPackage workPackage);
}
