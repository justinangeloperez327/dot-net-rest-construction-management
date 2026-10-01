using Construction.Domain.Activities;

namespace Construction.Application.Abstractions.Data;

public interface IActivityDependencyRepository
{
    Task<ActivityDependency?> GetAsync(
        Guid activityId,
        Guid predecessorActivityId,
        CancellationToken cancellationToken = default);

    Task<ActivityDependency?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ActivityDependency>> GetByActivityAsync(
        Guid activityId,
        CancellationToken cancellationToken = default);

    Task<bool> WouldCreateCycleAsync(
        Guid activityId,
        Guid predecessorActivityId,
        CancellationToken cancellationToken = default);

    void Add(ActivityDependency dependency);

    void Remove(ActivityDependency dependency);
}
