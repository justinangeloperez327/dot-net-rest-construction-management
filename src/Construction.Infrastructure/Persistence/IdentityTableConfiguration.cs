using Construction.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence;

internal static class IdentityTableConfiguration
{
    private const int IdentitySchemaVersion2KeyLength = 128;
    private const int IdentitySchemaVersion2PhoneNumberLength = 256;

    public static void Apply(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(builder =>
        {
            builder.ToTable("auth_users");

            builder.Property(user => user.PhoneNumber)
                .HasMaxLength(IdentitySchemaVersion2PhoneNumberLength);
        });

        modelBuilder.Entity<ApplicationRole>()
            .ToTable("auth_roles");

        modelBuilder.Entity<IdentityUserRole<Guid>>()
            .ToTable("auth_user_roles");

        modelBuilder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("auth_user_claims");

        modelBuilder.Entity<IdentityUserLogin<Guid>>(builder =>
        {
            builder.ToTable("auth_user_logins");

            builder.Property(login => login.LoginProvider)
                .HasMaxLength(IdentitySchemaVersion2KeyLength);

            builder.Property(login => login.ProviderKey)
                .HasMaxLength(IdentitySchemaVersion2KeyLength);
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("auth_role_claims");

        modelBuilder.Entity<IdentityUserToken<Guid>>(builder =>
        {
            builder.ToTable("auth_user_tokens");

            builder.Property(token => token.LoginProvider)
                .HasMaxLength(IdentitySchemaVersion2KeyLength);

            builder.Property(token => token.Name)
                .HasMaxLength(IdentitySchemaVersion2KeyLength);
        });
    }
}
