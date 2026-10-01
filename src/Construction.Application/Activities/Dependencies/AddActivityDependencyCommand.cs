using Construction.Application.Common.Messaging;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.Dependencies;

public sealed record AddActivityDependencyCommand(
    Guid ProjectId,
    Guid ActivityId,
    Guid PredecessorActivityId,
    ActivityDependencyType Type,
    int LagDays) : ICommand<ActivityDependencyResponse>;
