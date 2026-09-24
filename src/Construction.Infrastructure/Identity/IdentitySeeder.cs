using System.Security.Claims;
using Construction.Application.Common.Authorization;
using Construction.Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Construction.Infrastructure.Identity;

public sealed class IdentitySeeder(
    RoleManager<ApplicationRole> roleManager)
{
    public async Task SeedAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationRole? administratorRole =
            await roleManager.FindByNameAsync(SystemRoles.Administrator);

        if (administratorRole is null)
        {
            administratorRole = new ApplicationRole
            {
                Id = Guid.CreateVersion7(),
                Name = SystemRoles.Administrator
            };

            IdentityResult createResult =
                await roleManager.CreateAsync(administratorRole);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Unable to create the Administrator role.");
            }
        }

        IList<Claim> existingClaims =
            await roleManager.GetClaimsAsync(administratorRole);

        var existingPermissions = existingClaims
            .Where(claim =>
                claim.Type == AuthorizationClaimTypes.Permission)
            .Select(claim => claim.Value)
            .ToHashSet(StringComparer.Ordinal);

        foreach (string permission in Permissions.All)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (existingPermissions.Contains(permission))
            {
                continue;
            }

            IdentityResult claimResult =
                await roleManager.AddClaimAsync(
                    administratorRole,
                    new Claim(
                        AuthorizationClaimTypes.Permission,
                        permission));

            if (!claimResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unable to assign permission '{permission}' to the Administrator role.");
            }
        }
    }
}
