namespace Construction.Api.Contracts.Activities;

public sealed record UpdateWorkPackageRequest(
    string Name,
    string? Description,
    Guid? ParentWorkPackageId);
