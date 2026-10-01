using Construction.Domain.Rfis;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class RfiCommentConfiguration
    : IEntityTypeConfiguration<RfiComment>
{
    public void Configure(EntityTypeBuilder<RfiComment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("rfi_comments");
        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Body)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(comment => new
        {
            comment.RfiId,
            comment.CreatedAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(comment => comment.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
