using Construction.Domain.Activities;

namespace Construction.Api.Contracts.Activities;

public sealed record AddActivityDependencyRequest(
    Guid PredecessorActivityId,
    ActivityDependencyType Type,
    int LagDays);
