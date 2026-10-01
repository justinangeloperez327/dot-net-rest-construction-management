using Construction.Domain.Common;

namespace Construction.Domain.Activities;

public sealed class ActivityDependency : AuditableEntity<Guid>
{
    private ActivityDependency()
        : base(Guid.Empty)
    {
    }

    private ActivityDependency(
        Guid id,
        Guid activityId,
        Guid predecessorActivityId,
        ActivityDependencyType type,
        int lagDays)
        : base(id)
    {
        ActivityId = activityId;
        PredecessorActivityId = predecessorActivityId;
        Type = type;
        LagDays = lagDays;
    }

    public Guid ActivityId { get; private set; }

    public Guid PredecessorActivityId { get; private set; }

    public ActivityDependencyType Type { get; private set; }

    public int LagDays { get; private set; }

    public static ActivityDependency Create(
        Guid activityId,
        Guid predecessorActivityId,
        ActivityDependencyType type,
        int lagDays)
    {
        if (activityId == Guid.Empty
            || predecessorActivityId == Guid.Empty)
        {
            throw new DomainException(
                "Activity identifiers are required.");
        }

        if (activityId == predecessorActivityId)
        {
            throw new DomainException(
                "An activity cannot depend on itself.");
        }

        if (lagDays is < -3650 or > 3650)
        {
            throw new DomainException(
                "Dependency lag must be between -3650 and 3650 days.");
        }

        return new ActivityDependency(
            Guid.CreateVersion7(),
            activityId,
            predecessorActivityId,
            type,
            lagDays);
    }
}
