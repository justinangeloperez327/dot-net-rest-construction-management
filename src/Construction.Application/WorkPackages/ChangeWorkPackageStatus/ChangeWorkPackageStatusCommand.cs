using Construction.Application.Common.Messaging;
using Construction.Domain.WorkPackages;

namespace Construction.Application.WorkPackages.ChangeWorkPackageStatus;

public sealed record ChangeWorkPackageStatusCommand(
    Guid ProjectId,
    Guid WorkPackageId,
    WorkPackageStatus Status) : ICommand<WorkPackageResponse>;
