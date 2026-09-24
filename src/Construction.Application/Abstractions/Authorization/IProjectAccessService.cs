namespace Construction.Application.Abstractions.Authorization;

/// <summary>
/// Defines resource-level authorization for project-scoped operations.
/// The implementation will be added when project membership persistence exists.
/// </summary>
public interface IProjectAccessService
{
    Task<bool> HasProjectAccessAsync(
        Guid userId,
        Guid projectId,
        string permission,
        CancellationToken cancellationToken = default);
}
