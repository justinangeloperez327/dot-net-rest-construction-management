using Construction.Domain.WorkPackages;

namespace Construction.Api.Contracts.Activities;

public sealed record ChangeWorkPackageStatusRequest(
    WorkPackageStatus Status);
