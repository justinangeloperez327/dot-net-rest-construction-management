using Construction.Application.Common.Messaging;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.UpdateActivity;

public sealed record UpdateActivityCommand(
    Guid ProjectId,
    Guid ActivityId,
    string Name,
    string? Description,
    Guid? WorkPackageId,
    Guid? LocationId,
    ActivityPriority Priority,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate) : ICommand<ActivityResponse>;
