using Construction.Domain.Activities;

namespace Construction.Application.Activities;

public sealed record ActivityResponse(
    Guid Id,
    Guid ProjectId,
    string Code,
    string Name,
    string? Description,
    Guid? WorkPackageId,
    Guid? LocationId,
    ActivityPriority Priority,
    ActivityStatus Status,
    decimal ProgressPercentage,
    DateOnly? PlannedStartDate,
    DateOnly? PlannedEndDate,
    DateOnly? ActualStartDate,
    DateOnly? ActualEndDate,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static ActivityResponse FromDomain(Activity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        return new ActivityResponse(
            activity.Id,
            activity.ProjectId,
            activity.Code,
            activity.Name,
            activity.Description,
            activity.WorkPackageId,
            activity.LocationId,
            activity.Priority,
            activity.Status,
            activity.ProgressPercentage,
            activity.PlannedStartDate,
            activity.PlannedEndDate,
            activity.ActualStartDate,
            activity.ActualEndDate,
            activity.CreatedAtUtc,
            activity.LastModifiedAtUtc);
    }
}
