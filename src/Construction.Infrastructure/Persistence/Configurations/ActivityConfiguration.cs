using Construction.Domain.Activities;
using Construction.Domain.Locations;
using Construction.Domain.Projects;
using Construction.Domain.WorkPackages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Construction.Infrastructure.Persistence.Configurations;

public sealed class ActivityConfiguration
    : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("activities");
        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(activity => activity.NormalizedCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(activity => activity.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(activity => activity.Description)
            .HasMaxLength(4000);

        builder.Property(activity => activity.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(activity => activity.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(activity => activity.ProgressPercentage)
            .HasPrecision(5, 2);

        builder.Property(activity => activity.CreatedAtUtc)
            .IsRequired();

        builder.HasIndex(activity => new
        {
            activity.ProjectId,
            activity.NormalizedCode
        }).IsUnique();

        builder.HasIndex(activity => new
        {
            activity.ProjectId,
            activity.Status
        });

        builder.HasIndex(activity => activity.WorkPackageId);
        builder.HasIndex(activity => activity.LocationId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(activity => activity.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<WorkPackage>()
            .WithMany()
            .HasForeignKey(activity => activity.WorkPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProjectLocation>()
            .WithMany()
            .HasForeignKey(activity => activity.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
