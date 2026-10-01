using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Quality;

public static class QualityReferenceValidator
{
    public static async Task<ApplicationError?> ValidateLocationAndActivityAsync(
        Guid projectId,
        Guid? locationId,
        Guid? activityId,
        IProjectLocationRepository locations,
        IActivityRepository activities,
        string errorPrefix,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(locations);
        ArgumentNullException.ThrowIfNull(activities);
        ArgumentException.ThrowIfNullOrWhiteSpace(errorPrefix);

        if (locationId is Guid requestedLocationId)
        {
            var location = await locations.GetByIdAsync(
                requestedLocationId,
                cancellationToken);

            if (location is null || location.ProjectId != projectId)
            {
                return ApplicationError.Validation(
                    $"{errorPrefix}.InvalidLocation",
                    "The location must belong to the same project.");
            }
        }

        if (activityId is Guid requestedActivityId)
        {
            var activity = await activities.GetAsync(
                requestedActivityId,
                cancellationToken);

            if (activity is null || activity.ProjectId != projectId)
            {
                return ApplicationError.Validation(
                    $"{errorPrefix}.InvalidActivity",
                    "The activity must belong to the same project.");
            }
        }

        return null;
    }

    public static async Task<ApplicationError?> ValidateProjectMemberAsync(
        Guid projectId,
        Guid? userId,
        IProjectMemberRepository members,
        string errorCode,
        string description,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(members);

        if (userId is not Guid requestedUserId)
        {
            return null;
        }

        var member = await members.GetAsync(
            projectId,
            requestedUserId,
            cancellationToken);

        return member is null || !member.IsActive
            ? ApplicationError.Validation(errorCode, description)
            : null;
    }
}
