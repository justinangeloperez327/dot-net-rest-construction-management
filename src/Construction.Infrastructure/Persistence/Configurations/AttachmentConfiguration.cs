using Construction.Domain.Attachments;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class AttachmentConfiguration
    : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("attachments");
        builder.HasKey(attachment => attachment.Id);

        builder.Property(attachment => attachment.TargetType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(attachment => attachment.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(attachment => attachment.ContentType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(attachment => attachment.StorageKey)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(attachment => attachment.Description)
            .HasMaxLength(2000);

        builder.Property(attachment => attachment.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(attachment => new
        {
            attachment.ProjectId,
            attachment.TargetType,
            attachment.TargetId
        });

        builder.HasIndex(attachment => attachment.StorageKey)
            .IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(attachment => attachment.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(attachment => attachment.UploadedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
