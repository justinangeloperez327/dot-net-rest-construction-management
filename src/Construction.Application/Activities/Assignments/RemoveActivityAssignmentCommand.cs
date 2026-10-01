using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.Assignments;

public sealed record RemoveActivityAssignmentCommand(
    Guid ProjectId,
    Guid ActivityId,
    Guid AssignmentId) : ICommand;
