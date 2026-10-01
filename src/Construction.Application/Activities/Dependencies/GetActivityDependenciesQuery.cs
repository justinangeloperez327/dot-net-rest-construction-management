using Construction.Application.Common.Messaging;

namespace Construction.Application.Activities.Dependencies;

public sealed record GetActivityDependenciesQuery(
    Guid ProjectId,
    Guid ActivityId)
    : IQuery<IReadOnlyCollection<ActivityDependencyResponse>>;
