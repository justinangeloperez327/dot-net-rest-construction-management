using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Property(user => user.DisplayName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();
    }
}
