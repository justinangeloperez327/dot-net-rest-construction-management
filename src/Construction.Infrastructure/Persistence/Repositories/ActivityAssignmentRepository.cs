using Construction.Application.Abstractions.Data;
using Construction.Domain.Activities;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ActivityAssignmentRepository(ApplicationDbContext dbContext)
    : IActivityAssignmentRepository
{
    public Task<ActivityAssignment?> GetAsync(
        Guid activityId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.ActivityAssignments.SingleOrDefaultAsync(
            assignment =>
                assignment.ActivityId == activityId
                && assignment.UserId == userId,
            cancellationToken);

    public Task<ActivityAssignment?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.ActivityAssignments.SingleOrDefaultAsync(
            assignment => assignment.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<ActivityAssignment>> GetByActivityAsync(
        Guid activityId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ActivityAssignments
            .AsNoTracking()
            .Where(assignment => assignment.ActivityId == activityId)
            .OrderBy(assignment => assignment.Role)
            .ThenBy(assignment => assignment.UserId)
            .ToArrayAsync(cancellationToken);

    public void Add(ActivityAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        dbContext.ActivityAssignments.Add(assignment);
    }

    public void Remove(ActivityAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);
        dbContext.ActivityAssignments.Remove(assignment);
    }
}
