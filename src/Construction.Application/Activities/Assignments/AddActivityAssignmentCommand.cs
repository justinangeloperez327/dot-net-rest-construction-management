using Construction.Application.Common.Messaging;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.Assignments;

public sealed record AddActivityAssignmentCommand(
    Guid ProjectId,
    Guid ActivityId,
    Guid UserId,
    ActivityAssignmentRole Role) : ICommand<ActivityAssignmentResponse>;
