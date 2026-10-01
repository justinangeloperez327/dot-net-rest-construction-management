using Construction.Domain.Activities;

namespace Construction.Application.Abstractions.Data;

public interface IActivityAssignmentRepository
{
    Task<ActivityAssignment?> GetAsync(
        Guid activityId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ActivityAssignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ActivityAssignment>> GetByActivityAsync(
        Guid activityId,
        CancellationToken cancellationToken = default);

    void Add(ActivityAssignment assignment);

    void Remove(ActivityAssignment assignment);
}
