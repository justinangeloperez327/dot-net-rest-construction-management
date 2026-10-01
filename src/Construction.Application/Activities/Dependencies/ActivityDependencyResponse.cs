using Construction.Domain.Activities;

namespace Construction.Application.Activities.Dependencies;

public sealed record ActivityDependencyResponse(
    Guid Id,
    Guid ActivityId,
    Guid PredecessorActivityId,
    ActivityDependencyType Type,
    int LagDays,
    DateTimeOffset CreatedAtUtc)
{
    public static ActivityDependencyResponse FromDomain(
        ActivityDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);

        return new ActivityDependencyResponse(
            dependency.Id,
            dependency.ActivityId,
            dependency.PredecessorActivityId,
            dependency.Type,
            dependency.LagDays,
            dependency.CreatedAtUtc);
    }
}
