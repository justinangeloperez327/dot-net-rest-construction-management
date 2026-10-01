using Construction.Application.Common.Messaging;

namespace Construction.Application.WorkPackages.CreateWorkPackage;

public sealed record CreateWorkPackageCommand(
    Guid ProjectId,
    string Code,
    string Name,
    string? Description,
    Guid? ParentWorkPackageId) : ICommand<WorkPackageResponse>;
