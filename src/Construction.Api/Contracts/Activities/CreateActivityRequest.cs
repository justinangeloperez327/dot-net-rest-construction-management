using Construction.Domain.Activities;

namespace Construction.Api.Contracts.Activities;

public sealed record CreateActivityRequest(
    string Code,
    string Name,
    string? Description,
    Guid? WorkPackageId,
    Guid? LocationId,
    ActivityPriority Priority,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate);
