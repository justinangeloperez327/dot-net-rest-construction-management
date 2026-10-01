using Construction.Application.Common.Messaging;

namespace Construction.Application.WorkPackages.UpdateWorkPackage;

public sealed record UpdateWorkPackageCommand(
    Guid ProjectId,
    Guid WorkPackageId,
    string Name,
    string? Description,
    Guid? ParentWorkPackageId) : ICommand<WorkPackageResponse>;
