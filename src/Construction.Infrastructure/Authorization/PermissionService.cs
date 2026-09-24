using System.Security.Claims;
using Construction.Application.Abstractions.Authorization;
using Construction.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Construction.Infrastructure.Authorization;

public sealed class PermissionService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager)
    : IPermissionService
{
    public async Task<bool> HasPermissionAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);

        IReadOnlySet<string> permissions =
            await GetPermissionsAsync(userId, cancellationToken);

        return permissions.Contains(permission);
    }

    public async Task<IReadOnlySet<string>> GetPermissionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUser? user =
            await userManager.FindByIdAsync(userId.ToString());

        if (user is null || !user.IsActive)
        {
            return new HashSet<string>(StringComparer.Ordinal);
        }

        var permissions = new HashSet<string>(StringComparer.Ordinal);

        IList<Claim> userClaims = await userManager.GetClaimsAsync(user);

        foreach (Claim claim in userClaims)
        {
            if (claim.Type == AuthorizationClaimTypes.Permission)
            {
                permissions.Add(claim.Value);
            }
        }

        IList<string> roleNames = await userManager.GetRolesAsync(user);

        foreach (string roleName in roleNames)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ApplicationRole? role =
                await roleManager.FindByNameAsync(roleName);

            if (role is null)
            {
                continue;
            }

            IList<Claim> roleClaims =
                await roleManager.GetClaimsAsync(role);

            foreach (Claim claim in roleClaims)
            {
                if (claim.Type == AuthorizationClaimTypes.Permission)
                {
                    permissions.Add(claim.Value);
                }
            }
        }

        return permissions;
    }
}
