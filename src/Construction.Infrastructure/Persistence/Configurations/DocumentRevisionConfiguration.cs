using Construction.Domain.Documents;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class DocumentRevisionConfiguration
    : IEntityTypeConfiguration<DocumentRevision>
{
    public void Configure(EntityTypeBuilder<DocumentRevision> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("document_revisions");
        builder.HasKey(revision => revision.Id);

        builder.Property(revision => revision.RevisionCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(revision => revision.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(revision => revision.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(revision => revision.StorageKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(revision => revision.Notes)
            .HasMaxLength(2000);

        builder.Property(revision => revision.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(revision => new
        {
            revision.DocumentId,
            revision.VersionNumber
        }).IsUnique();

        builder.HasIndex(revision => new
        {
            revision.DocumentId,
            revision.RevisionCode
        }).IsUnique();

        builder.HasIndex(revision => new
        {
            revision.DocumentId,
            revision.IsCurrent
        });

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(revision => revision.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
