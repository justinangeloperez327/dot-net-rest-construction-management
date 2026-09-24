using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Common.Errors;

namespace Construction.Application.Common.Authorization;

public static class ProjectAccessGuard
{
    public static async Task<ApplicationError?> CheckAsync(
        ICurrentUser currentUser,
        IProjectAccessService projectAccessService,
        Guid projectId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(currentUser);
        ArgumentNullException.ThrowIfNull(projectAccessService);
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);

        if (!currentUser.IsAuthenticated
            || currentUser.UserId is not Guid userId)
        {
            return ApplicationError.Unauthorized(
                "Authorization.Unauthenticated",
                "Authentication is required.");
        }

        bool allowed = await projectAccessService.HasProjectAccessAsync(
            userId,
            projectId,
            permission,
            cancellationToken);

        return allowed
            ? null
            : ApplicationError.Forbidden(
                "Authorization.ProjectAccessDenied",
                "You do not have access to perform this operation on the project.");
    }
}
