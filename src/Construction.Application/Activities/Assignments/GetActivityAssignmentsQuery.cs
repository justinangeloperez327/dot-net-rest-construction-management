using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.Assignments;

public sealed record GetActivityAssignmentsQuery(
    Guid ProjectId,
    Guid ActivityId)
    : IQuery<IReadOnlyCollection<ActivityAssignmentResponse>>;
