using Construction.Application.Common.Messaging;

namespace Construction.Application.WorkPackages.GetWorkPackage;

public sealed record GetWorkPackageQuery(
    Guid ProjectId,
    Guid WorkPackageId) : IQuery<WorkPackageResponse>;
