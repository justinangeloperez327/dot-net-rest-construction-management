using Construction.Application.Common.Messaging;

namespace Construction.Application.WorkPackages.GetWorkPackages;

public sealed record GetWorkPackagesQuery(Guid ProjectId)
    : IQuery<IReadOnlyCollection<WorkPackageResponse>>;
