using Construction.Domain.Projects;
using Construction.Domain.Submittals;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class SubmittalConfiguration
    : IEntityTypeConfiguration<Submittal>
{
    public void Configure(EntityTypeBuilder<Submittal> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("submittals");
        builder.HasKey(submittal => submittal.Id);

        builder.Property(submittal => submittal.Number)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(submittal => submittal.NormalizedNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(submittal => submittal.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(submittal => submittal.Type)
            .HasConversion<string>()
            .HasMaxLength(40);

        builder.Property(submittal => submittal.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(submittal => submittal.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(submittal => new
        {
            submittal.ProjectId,
            submittal.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(submittal => new
        {
            submittal.ProjectId,
            submittal.Status,
            submittal.Type
        });

        builder.HasIndex(submittal => submittal.ResponsibleUserId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(submittal => submittal.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(submittal => submittal.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(submittal => submittal.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(submittal => submittal.Revisions)
            .WithOne()
            .HasForeignKey(revision => revision.SubmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(submittal => submittal.Comments)
            .WithOne()
            .HasForeignKey(comment => comment.SubmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(submittal => submittal.History)
            .WithOne()
            .HasForeignKey(entry => entry.SubmittalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(submittal => submittal.Revisions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(submittal => submittal.Comments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(submittal => submittal.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
