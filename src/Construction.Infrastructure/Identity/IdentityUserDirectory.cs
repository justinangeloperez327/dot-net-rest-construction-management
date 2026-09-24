using Construction.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Construction.Infrastructure.Identity;

public sealed class IdentityUserDirectory(
    UserManager<ApplicationUser> userManager)
    : IUserDirectory
{
    public async Task<bool> ExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApplicationUser? user =
            await userManager.FindByIdAsync(userId.ToString());

        return user is not null && user.IsActive;
    }
}
