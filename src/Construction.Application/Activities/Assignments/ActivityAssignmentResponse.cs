using Construction.Domain.Activities;

namespace Construction.Application.Activities.Assignments;

public sealed record ActivityAssignmentResponse(
    Guid Id,
    Guid ActivityId,
    Guid UserId,
    ActivityAssignmentRole Role,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static ActivityAssignmentResponse FromDomain(
        ActivityAssignment assignment)
    {
        ArgumentNullException.ThrowIfNull(assignment);

        return new ActivityAssignmentResponse(
            assignment.Id,
            assignment.ActivityId,
            assignment.UserId,
            assignment.Role,
            assignment.CreatedAtUtc,
            assignment.LastModifiedAtUtc);
    }
}
