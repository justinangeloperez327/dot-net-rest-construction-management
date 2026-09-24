using Construction.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence;

internal static class IdentityTableConfiguration
{
    public static void Apply(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ApplicationUser>().ToTable("auth_users");
        modelBuilder.Entity<ApplicationRole>().ToTable("auth_roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("auth_user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("auth_user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("auth_user_logins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("auth_role_claims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("auth_user_tokens");
    }
}
