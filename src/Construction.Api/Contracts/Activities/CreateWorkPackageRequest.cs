namespace Construction.Api.Contracts.Activities;

public sealed record CreateWorkPackageRequest(
    string Code,
    string Name,
    string? Description,
    Guid? ParentWorkPackageId);
