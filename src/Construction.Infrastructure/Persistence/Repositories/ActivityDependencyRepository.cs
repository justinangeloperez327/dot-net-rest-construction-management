using Construction.Application.Abstractions.Data;
using Construction.Domain.Activities;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ActivityDependencyRepository(ApplicationDbContext dbContext)
    : IActivityDependencyRepository
{
    public Task<ActivityDependency?> GetAsync(
        Guid activityId,
        Guid predecessorActivityId,
        CancellationToken cancellationToken = default) =>
        dbContext.ActivityDependencies.SingleOrDefaultAsync(
            dependency =>
                dependency.ActivityId == activityId
                && dependency.PredecessorActivityId == predecessorActivityId,
            cancellationToken);

    public Task<ActivityDependency?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.ActivityDependencies.SingleOrDefaultAsync(
            dependency => dependency.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<ActivityDependency>> GetByActivityAsync(
        Guid activityId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ActivityDependencies
            .AsNoTracking()
            .Where(dependency => dependency.ActivityId == activityId)
            .OrderBy(dependency => dependency.PredecessorActivityId)
            .ToArrayAsync(cancellationToken);

    public async Task<bool> WouldCreateCycleAsync(
        Guid activityId,
        Guid predecessorActivityId,
        CancellationToken cancellationToken = default)
    {
        var pending = new Stack<Guid>();
        var visited = new HashSet<Guid>();

        pending.Push(predecessorActivityId);

        while (pending.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Guid current = pending.Pop();

            if (current == activityId)
            {
                return true;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            Guid[] predecessors = await dbContext.ActivityDependencies
                .AsNoTracking()
                .Where(dependency => dependency.ActivityId == current)
                .Select(dependency => dependency.PredecessorActivityId)
                .ToArrayAsync(cancellationToken);

            foreach (Guid predecessor in predecessors)
            {
                pending.Push(predecessor);
            }
        }

        return false;
    }

    public void Add(ActivityDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        dbContext.ActivityDependencies.Add(dependency);
    }

    public void Remove(ActivityDependency dependency)
    {
        ArgumentNullException.ThrowIfNull(dependency);
        dbContext.ActivityDependencies.Remove(dependency);
    }
}
