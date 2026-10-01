using Construction.Domain.Documents;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ProjectDocumentConfiguration
    : IEntityTypeConfiguration<ProjectDocument>
{
    public void Configure(EntityTypeBuilder<ProjectDocument> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("documents");
        builder.HasKey(document => document.Id);

        builder.Property(document => document.Number)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(document => document.NormalizedNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(document => document.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(document => document.Category)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(document => document.Description)
            .HasMaxLength(4000);

        builder.Property(document => document.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(document => document.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(document => new
        {
            document.ProjectId,
            document.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(document => new
        {
            document.ProjectId,
            document.Category,
            document.Status
        });

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(document => document.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(document => document.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(document => document.Revisions)
            .WithOne()
            .HasForeignKey(revision => revision.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(document => document.Revisions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
