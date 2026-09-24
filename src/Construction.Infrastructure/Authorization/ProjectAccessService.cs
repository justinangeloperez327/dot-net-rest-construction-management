using Construction.Application.Abstractions.Authorization;
using Construction.Application.Common.Authorization;
using Construction.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Authorization;

public sealed class ProjectAccessService(
    IPermissionService permissionService,
    ApplicationDbContext dbContext)
    : IProjectAccessService
{
    public async Task<bool> HasProjectAccessAsync(
        Guid userId,
        Guid projectId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        IReadOnlySet<string> permissions =
            await permissionService.GetPermissionsAsync(
                userId,
                cancellationToken);

        if (!permissions.Contains(permission))
        {
            return false;
        }

        if (permissions.Contains(Permissions.Projects.AccessAll))
        {
            return true;
        }

        return await dbContext.ProjectMembers
            .AsNoTracking()
            .AnyAsync(
                member =>
                    member.ProjectId == projectId
                    && member.UserId == userId
                    && member.IsActive,
                cancellationToken);
    }
}
