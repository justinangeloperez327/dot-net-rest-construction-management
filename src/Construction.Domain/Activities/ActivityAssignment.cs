using Construction.Domain.Common;

namespace Construction.Domain.Activities;

public sealed class ActivityAssignment : AuditableEntity<Guid>
{
    private ActivityAssignment()
        : base(Guid.Empty)
    {
    }

    private ActivityAssignment(
        Guid id,
        Guid activityId,
        Guid userId,
        ActivityAssignmentRole role)
        : base(id)
    {
        ActivityId = activityId;
        UserId = userId;
        Role = role;
    }

    public Guid ActivityId { get; private set; }

    public Guid UserId { get; private set; }

    public ActivityAssignmentRole Role { get; private set; }

    public static ActivityAssignment Create(
        Guid activityId,
        Guid userId,
        ActivityAssignmentRole role)
    {
        if (activityId == Guid.Empty)
        {
            throw new DomainException("Activity identifier is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("User identifier is required.");
        }

        return new ActivityAssignment(
            Guid.CreateVersion7(),
            activityId,
            userId,
            role);
    }

    public void ChangeRole(ActivityAssignmentRole role)
    {
        Role = role;
    }
}
