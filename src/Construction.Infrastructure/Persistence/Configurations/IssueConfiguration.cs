using Construction.Domain.Activities;
using Construction.Domain.Issues;
using Construction.Domain.Locations;
using Construction.Domain.Projects;
using Construction.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("issues");
        builder.HasKey(issue => issue.Id);

        builder.Property(issue => issue.Number)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(issue => issue.NormalizedNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(issue => issue.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(issue => issue.Description)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(issue => issue.Type)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(issue => issue.Severity)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(issue => issue.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(issue => issue.ResolutionSummary)
            .HasMaxLength(5000);

        builder.Property(issue => issue.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(issue => new
        {
            issue.ProjectId,
            issue.NormalizedNumber
        }).IsUnique();

        builder.HasIndex(issue => new
        {
            issue.ProjectId,
            issue.Status,
            issue.Severity,
            issue.DueDate
        });

        builder.HasIndex(issue => issue.LocationId);
        builder.HasIndex(issue => issue.ActivityId);
        builder.HasIndex(issue => issue.ResponsibleUserId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(issue => issue.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ProjectLocation>()
            .WithMany()
            .HasForeignKey(issue => issue.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Activity>()
            .WithMany()
            .HasForeignKey(issue => issue.ActivityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(issue => issue.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(issue => issue.ResponsibleUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(issue => issue.VerifiedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(issue => issue.CorrectiveActions)
            .WithOne()
            .HasForeignKey(action => action.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(issue => issue.History)
            .WithOne()
            .HasForeignKey(entry => entry.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(issue => issue.CorrectiveActions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(issue => issue.History)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
