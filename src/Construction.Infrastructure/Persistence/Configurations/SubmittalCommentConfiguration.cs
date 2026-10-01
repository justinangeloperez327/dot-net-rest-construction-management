using Construction.Domain.Submittals;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class SubmittalCommentConfiguration
    : IEntityTypeConfiguration<SubmittalComment>
{
    public void Configure(EntityTypeBuilder<SubmittalComment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("submittal_comments");
        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.Body)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(comment => new
        {
            comment.SubmittalId,
            comment.CreatedAtUtc
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(comment => comment.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
