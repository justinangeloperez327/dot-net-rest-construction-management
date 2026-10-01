using Construction.Domain.WorkPackages;

namespace Construction.Application.WorkPackages;

public sealed record WorkPackageResponse(
    Guid Id,
    Guid ProjectId,
    string Code,
    string Name,
    string? Description,
    Guid? ParentWorkPackageId,
    WorkPackageStatus Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static WorkPackageResponse FromDomain(WorkPackage workPackage)
    {
        ArgumentNullException.ThrowIfNull(workPackage);

        return new WorkPackageResponse(
            workPackage.Id,
            workPackage.ProjectId,
            workPackage.Code,
            workPackage.Name,
            workPackage.Description,
            workPackage.ParentWorkPackageId,
            workPackage.Status,
            workPackage.CreatedAtUtc,
            workPackage.LastModifiedAtUtc);
    }
}
