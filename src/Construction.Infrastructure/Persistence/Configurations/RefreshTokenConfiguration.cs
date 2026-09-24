using Construction.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("auth_refresh_tokens");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.HasIndex(token => token.TokenHash)
            .IsUnique();

        builder.Property(token => token.ReplacedByTokenHash)
            .HasMaxLength(64);

        builder.HasIndex(token => new
        {
            token.UserId,
            token.RevokedAtUtc,
            token.ExpiresAtUtc
        });

        builder.HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
