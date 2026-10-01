using Construction.Application.Common.Messaging;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.CreateActivity;

public sealed record CreateActivityCommand(
    Guid ProjectId,
    string Code,
    string Name,
    string? Description,
    Guid? WorkPackageId,
    Guid? LocationId,
    ActivityPriority Priority,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate) : ICommand<ActivityResponse>;
