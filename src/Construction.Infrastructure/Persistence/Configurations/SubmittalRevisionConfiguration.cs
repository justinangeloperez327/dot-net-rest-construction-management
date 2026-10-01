using Construction.Domain.Submittals;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class SubmittalRevisionConfiguration
    : IEntityTypeConfiguration<SubmittalRevision>
{
    public void Configure(EntityTypeBuilder<SubmittalRevision> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("submittal_revisions");
        builder.HasKey(revision => revision.Id);

        builder.Property(revision => revision.RevisionCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(revision => revision.Description)
            .HasMaxLength(4000);

        builder.Property(revision => revision.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(revision => revision.ReviewRemarks)
            .HasMaxLength(4000);

        builder.Property(revision => revision.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(revision => new
        {
            revision.SubmittalId,
            revision.VersionNumber
        }).IsUnique();

        builder.HasIndex(revision => new
        {
            revision.SubmittalId,
            revision.RevisionCode
        }).IsUnique();

        builder.HasIndex(revision => new
        {
            revision.SubmittalId,
            revision.IsCurrent
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(revision => revision.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(revision => revision.SubmittedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(revision => revision.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
